using JameGam.Common;
using JameGam.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

namespace JameGam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameObject _playerPrefab, _networkPlayerPrefab;

        [SerializeField]
        private GameObject _objectiveUI;

        [SerializeField]
        private TMP_Text _objectiveText;

        [SerializeField]
        private TMP_FontAsset _killFont;

        [SerializeField]
        private GameObject _menu;

        [SerializeField]
        private AudioSource _source;

        private PlayerController _player;

        private TcpClient _tcp;

        private readonly Dictionary<int, Player.NetworkPlayer> _networkPlayers = new();
        private readonly List<(int, string)> _toInstantiate = new();

        private Thread _networkThread;

        private int _localPlayerNetworkID = -1;

        private bool _awaitingUIReset;

        private bool _isSolo;
        public bool IsSolo => _isSolo;

        private int _soloAI;
        public int SoloAI
        {
            set
            {
                _soloAI = value;
                if (_soloAI == 0)
                {
                    _objectiveText.text = "You are the best elf";
                    _objectiveText.color = Color.green;
                }
            }
            get => _soloAI;
        }

        public bool DidAIDie { set; get; }

        public void PlaySolo()
        {
            _isSolo = true;

            SceneManager.LoadScene("Solo", LoadSceneMode.Additive);
            _player = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<PlayerController>();

            _source.Play();
            ShowGameUI();
            _menu.SetActive(false);
        }

        public bool Connect(string ip, int port, string name)
        {
            try
            {
                if (name.Length > 20) name = name[..20];

                _tcp = new TcpClient();
                _tcp.Connect(ip, port);

                using MemoryStream ms = new();
                using BinaryWriter writer = new(ms);

                writer.Write((ushort)MessageType.Handshake);
                writer.Write((ushort)1);
                writer.Write(name);

                var data = ms.ToArray();
                _tcp.GetStream().Write(data, 0, data.Length);

                _networkThread = new Thread(new ThreadStart(ListenIncomingMessages));
                _networkThread.Start();

                _source.Play();

                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return false;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (_toInstantiate.Any())
            {
                lock(_toInstantiate)
                {
                    foreach (var e in _toInstantiate)
                    {
                        var p = Instantiate(_networkPlayerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Player.NetworkPlayer>();
                        p.GetComponentInChildren<TMP_Text>().text = e.Item2;
                        p.NetID = e.Item1;
                        _networkPlayers[e.Item1] = p;
                    }
                    _toInstantiate.Clear();
                }
            }

            if (_player == null && _localPlayerNetworkID != -1)
            {
                _player = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<PlayerController>();
                ShowGameUI();
            }

            if (_awaitingUIReset)
            {
                _awaitingUIReset = false;
                ShowGameUI();
            }
        }

        public void UpdateObjectiveDead()
        {
            if (_isSolo && SoloAI == 0)
            {
                _objectiveText.text = "You <i>were</i> the best elf";
                return;
            }

            if (_isSolo) _objectiveText.text = "You lost";
            else _objectiveText.text = "Waiting for game to end...";
        }

        public void ShowGameUI()
        {
            _objectiveUI.SetActive(true);
            _objectiveText.text = "Mine some rocks";
        }

        public void UpdateObjectiveCraft()
            => _objectiveText.text = "Craft a fun sword toy";

        public void UpdateObjectiveSword()
        {
            _objectiveText.text = "Kill them all";
            _objectiveText.color = Color.red;
            _objectiveText.font = _killFont;
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void ListenIncomingMessages()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    _tcp.GetStream().Read(buffer, 0, 1024);

                    using MemoryStream ms = new(buffer);
                    using BinaryReader reader = new(ms);

                    while (reader.PeekChar() != -1)
                    {
                        var msg = (MessageType)reader.ReadUInt16();
                        switch (msg)
                        {
                            case MessageType.Handshake:
                                {
                                    _localPlayerNetworkID = reader.ReadInt32();
                                }
                                break;

                            case MessageType.Connected:
                                {
                                    var id = reader.ReadInt32();

                                    if (_networkPlayers.ContainsKey(id))
                                    {
                                        Debug.LogWarning($"Received connection message for a player already connected: {id}");
                                    }
                                    else
                                    {
                                        _networkPlayers.Add(id, null);
                                        lock (_toInstantiate)
                                        {
                                            _toInstantiate.Add((id, reader.ReadString()));
                                        }
                                        Debug.LogWarning($"New player registered with ID {id}");
                                    }
                                }
                                break;

                            case MessageType.SpacialInfo:
                                {
                                    var id = reader.ReadInt32();

                                    if (!_networkPlayers.ContainsKey(id))
                                    {
                                        Debug.LogWarning($"Received message for a player not connected: {id}");
                                    }
                                    else if (_networkPlayers[id] == null)
                                    { } // Player not instanciated yet
                                    else
                                    {
                                        var pos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                                        var vel = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                                        _networkPlayers[id].SetSpacialInfo(pos, vel);
                                    }
                                }
                                break;

                            case MessageType.Death:
                                {
                                    var id = reader.ReadInt32();

                                    if (id == _localPlayerNetworkID)
                                    {
                                        _player.SetDeathStatus(true);
                                    }
                                    else if (!_networkPlayers.ContainsKey(id))
                                    {
                                        Debug.LogWarning($"Received message for a player not connected: {id}");
                                    }
                                    else if (_networkPlayers[id] == null) // TODO
                                    { } // Player not instanciated yet
                                    else
                                    {
                                        _networkPlayers[id].SetDeathStatus(true);
                                    }
                                }
                                break;

                            case MessageType.ResetGame:
                                {
                                    _player?.ResetC();
                                    lock (_networkPlayers)
                                    {
                                        foreach (var c in _networkPlayers.Values)
                                        {
                                            c.ResetC();
                                        }
                                    }
                                    _awaitingUIReset = true;
                                }
                                break;

                            case MessageType.CarryChange:
                                {
                                    var id = reader.ReadInt32();

                                    if (!_networkPlayers.ContainsKey(id))
                                    {
                                        Debug.LogWarning($"Received message for a player not connected: {id}");
                                    }
                                    else if (_networkPlayers[id] == null)
                                    { } // Player not instanciated yet
                                    else
                                    {
                                        _networkPlayers[id].SetCarry((CarryType)reader.ReadInt16());
                                    }
                                }
                                break;

                            case MessageType.AttackAnim:
                                {
                                    var id = reader.ReadInt32();

                                    if (!_networkPlayers.ContainsKey(id))
                                    {
                                        Debug.LogWarning($"Received message for a player not connected: {id}");
                                    }
                                    else if (_networkPlayers[id] == null)
                                    { } // Player not instanciated yet
                                    else
                                    {
                                        _networkPlayers[id].SetAttackAnim();
                                    }
                                }
                                break;

                            case MessageType.Stunned:
                                {
                                    var id = reader.ReadInt32();
                                    var dir = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                                    if (id == _localPlayerNetworkID)
                                    {
                                        _player.SetStun(dir);
                                    }
                                    else if (!_networkPlayers.ContainsKey(id))
                                    {
                                        Debug.LogWarning($"Received message for a player not connected: {id}");
                                    }
                                    else if (_networkPlayers[id] == null)
                                    { } // Player not instanciated yet
                                    else
                                    {
                                        _networkPlayers[id].SetStun(Vector2.zero);
                                    }
                                }
                                break;

                            default:
                                Debug.LogWarning($"Unknown network message {msg}");
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                Thread.Sleep(10);
            }
        }

        public void SendDeath(int? id)
        {
            if (_isSolo) return;

            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);

            writer.Write((ushort)MessageType.Death);
            writer.Write(id ?? _localPlayerNetworkID);

            var data = ms.ToArray();
            _tcp.GetStream().Write(data, 0, data.Length);
        }

        public void SendCarry(CarryType carry)
        {
            if (_isSolo) return;

            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);

            writer.Write((ushort)MessageType.CarryChange);
            writer.Write((short)carry);

            var data = ms.ToArray();
            _tcp.GetStream().Write(data, 0, data.Length);
        }

        public void SendStun(int id, Vector2 dir)
        {
            if (_isSolo) return;

            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);

            writer.Write((ushort)MessageType.Stunned);
            writer.Write(id);
            writer.Write(dir.x);
            writer.Write(dir.y);

            var data = ms.ToArray();
            _tcp.GetStream().Write(data, 0, data.Length);
        }

        public void SendAttack()
        {
            if (_isSolo) return;

            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);

            writer.Write((ushort)MessageType.AttackAnim);

            var data = ms.ToArray();
            _tcp.GetStream().Write(data, 0, data.Length);
        }

        public void SendSpacialInfo(Vector2 pos, Vector2 vel)
        {
            if (_isSolo) return;

            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);

            writer.Write((ushort)MessageType.SpacialInfo);
            writer.Write(pos.x);
            writer.Write(pos.y);
            writer.Write(vel.x);
            writer.Write(vel.y);

            var data = ms.ToArray();
            _tcp.GetStream().Write(data, 0, data.Length);
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            if (_player != null)
            {
                _player.OnMove(value.ReadValue<Vector2>());
            }
        }

        public void OnAttack(InputAction.CallbackContext value)
        {
            if (_player != null && value.performed)
            {
                StartCoroutine(_player.OnAttack());
            }
        }

        private void OnDestroy()
        {
            if (_isSolo) return;
            _networkThread.Abort();
            if (_tcp.Connected)
            {
                using MemoryStream ms = new();
                using BinaryWriter writer = new(ms);

                writer.Write((ushort)MessageType.Disconnected);

                var data = ms.ToArray();
                _tcp.GetStream().Write(data, 0, data.Length);
            }
        }
    }
}
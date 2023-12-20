using JameGam.Common;
using JameGam.Player;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JameGam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameObject _playerPrefab, _networkPlayerPrefab;

        private PlayerController _player;

        private TcpClient _tcp;

        private readonly Dictionary<int, Player.NetworkPlayer> _networkPlayers = new();
        private readonly List<int> _toInstantiate = new();

        private void Awake()
        {
            Instance = this;
            try
            {
                _tcp = new TcpClient();
                _tcp.Connect("localhost", 9999);

                using MemoryStream ms = new();
                using BinaryWriter writer = new(ms);

                writer.Write((ushort)MessageType.Handshake);
                writer.Write((ushort)1);
                writer.Write("Player");

                var data = ms.ToArray();
                _tcp.GetStream().Write(data, 0, data.Length);

                new Thread(new ThreadStart(ListenIncomingMessages)).Start();

                _player = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<PlayerController>();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Update()
        {
            if (_toInstantiate.Any())
            {
                lock(_toInstantiate)
                {
                    foreach (var id in _toInstantiate)
                    {
                        var p = Instantiate(_networkPlayerPrefab, Vector2.zero, Quaternion.identity).GetComponent<Player.NetworkPlayer>();
                        _networkPlayers[id] = p;
                    }
                    _toInstantiate.Clear();
                }
            }
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

                    var msg = (MessageType)reader.ReadUInt16();
                    switch (msg)
                    {
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
                                        _toInstantiate.Add(id);
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

                        default:
                            Debug.LogWarning($"Unknown network message {msg}");
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
                Thread.Sleep(10);
            }
        }

        public void SendSpacialInfo(Vector2 pos, Vector2 vel)
        {
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
    }
}
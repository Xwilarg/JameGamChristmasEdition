using JameGam.Common;
using JameGam.Player;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JameGam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameObject _playerPrefab;

        private PlayerController _player;

        private TcpClient _tcp;

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


                _player = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity).GetComponent<PlayerController>();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
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
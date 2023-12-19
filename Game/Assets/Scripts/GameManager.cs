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
                writer.Write((short)MessageType.Connected);
                writer.Write("Player");

                var length = ms.ToArray().Length;

                using MemoryStream ms2 = new();
                using BinaryWriter writer2 = new(ms);

                writer2.Write(length);
                writer2.Write(ms.ToArray());

                var data = ms2.ToArray();
                _tcp.GetStream().Write(data, 0, data.Length);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            var p = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity);
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            if (_player != null)
            {
                _player.OnMove(value.ReadValue<Vector2>());

                using MemoryStream ms = new();
                using BinaryWriter writer = new(ms);

                writer.Write((short)MessageType.Connected);
                writer.Write(_player.transform.position.x);
                writer.Write(_player.transform.position.y);
                writer.Write(_player.Velocity.x);
                writer.Write(_player.Velocity.y);

                var length = ms.ToArray().Length;

                using MemoryStream ms2 = new();
                using BinaryWriter writer2 = new(ms);

                writer2.Write(length);
                writer2.Write(ms.ToArray());

                var data = ms2.ToArray();
                _tcp.GetStream().Write(data, 0, data.Length);
            }
        }
    }
}
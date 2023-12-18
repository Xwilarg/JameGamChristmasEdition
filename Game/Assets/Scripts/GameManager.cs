using JameGam.Player;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JameGam
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _playerPrefab;

        private PlayerController _player;

        private TcpClient _tcp;

        private void Awake()
        {
            try
            {
                _tcp = new TcpClient();
                _tcp.Connect("localhost", 9999);
            }
            catch (System.Exception e)
            {

            }

            var p = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity);
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            if (_player != null)
            {
                _player.OnMove(value.ReadValue<Vector2>());
            }
        }
    }
}
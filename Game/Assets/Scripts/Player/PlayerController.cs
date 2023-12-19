using UnityEngine;

namespace JameGam.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;

        private Vector2 _mov;

        public Vector2 Velocity => _rb.velocity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = _mov;
        }

        public void OnMove(Vector2 val)
        {
            _mov = val;
        }
    }
}

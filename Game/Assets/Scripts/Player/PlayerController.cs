using UnityEngine;

namespace JameGam.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;

        private Vector2 _mov;

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

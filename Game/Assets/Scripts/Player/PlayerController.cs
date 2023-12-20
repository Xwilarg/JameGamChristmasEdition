using UnityEngine;

namespace JameGam.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;

        private Vector2 _mov;

        public Vector2 Velocity => _rb.velocity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var targetPos = transform.position.y;
            _sr.sortingOrder = (int)(-targetPos * 100f);
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

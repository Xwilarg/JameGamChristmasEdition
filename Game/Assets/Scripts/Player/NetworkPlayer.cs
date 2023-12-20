using UnityEngine;

namespace JameGam.Player
{
    public class NetworkPlayer : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Animator _anim;

        private Vector2 _pos, _vel;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            transform.position = _pos;
            _rb.velocity = _vel;
            _anim.SetFloat("X", _vel.x);
            _anim.SetFloat("Y", _vel.y);
        }

        public void SetSpacialInfo(Vector2 pos, Vector2 vel)
        {
            _pos = pos;
            _vel = vel;
        }
    }
}

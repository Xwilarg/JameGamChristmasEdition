using UnityEngine;

namespace Assets.Scripts.Player
{
    public class NetworkPlayer : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
        }

        public void SetSpacialInfo(Vector2 pos, Vector2 vel)
        {
            transform.position = pos;
            _rb.velocity = vel;
            _anim.SetFloat("X", vel.x);
            _anim.SetFloat("Y", vel.y);
        }
    }
}

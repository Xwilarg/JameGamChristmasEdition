using UnityEngine;

namespace JameGam.Player
{
    public class NetworkPlayer : MonoBehaviour
    {
        public int NetworkID { set; get; }

        private Rigidbody2D _rb;
        private Animator _anim;

        private Vector2 _pos, _vel;
        private CarryType _carry;

        private bool _isDirty;
        private bool _isDead;

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

            if (_isDirty)
            {
                _isDirty = false;
                if (_carry == CarryType.None) _anim.SetInteger("Carrying", 0);
                else if (_carry == CarryType.Rock) _anim.SetInteger("Carrying", 1);
                else _anim.SetInteger("Carrying", 2);

                _anim.SetBool("IsDead", _isDead);
            }
        }

        public void SetSpacialInfo(Vector2 pos, Vector2 vel)
        {
            _pos = pos;
            _vel = vel;
        }

        public void SetCarry(CarryType carry)
        {
            _carry = carry;
            _isDirty = true;
        }

        public void SetDeathStatus(bool status)
        {
            _isDead = status;
            _isDirty = true;
        }
    }
}

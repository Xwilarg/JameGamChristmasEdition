using UnityEngine;

namespace JameGam.Player
{
    public class NetworkPlayer : ACharacter
    {
        public int NetworkID { set; get; }

        private Vector2 _pos, _vel;
        private CarryType _carry;

        private bool _isDirty;
        private bool _isDead;

        private void Awake()
        {
            AwakeParent();
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

            UpdateParent();
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

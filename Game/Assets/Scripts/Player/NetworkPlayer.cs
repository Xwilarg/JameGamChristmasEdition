using System.Collections;
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

        private bool _awaitingAttack;
        private bool _awaitingStun;

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

            if (_awaitingAttack)
            {
                _awaitingAttack = false;
                StartCoroutine(Attack());
            }

            if (_awaitingStun)
            {
                _awaitingStun = false;
                StartCoroutine(Stun());
            }

            UpdateParent();
        }

        private IEnumerator Attack()
        {
            _anim.SetBool("IsAttacking", true);
            yield return new WaitForSeconds(.75f);
            _anim.SetBool("IsAttacking", false);
        }

        private IEnumerator Stun()
        {
            _anim.SetBool("IsStunned", true);
            yield return new WaitForSeconds(1f);
            _anim.SetBool("IsStunned", false);
        }

        public override void ResetC()
        {
            _pos = Vector2.zero;
            _carry = CarryType.None;
            _isDead = false;
            _isDirty = true;
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

        public void SetAttackAnim()
        {
            _awaitingAttack = true;
        }

        public void SetStun()
        {
            _awaitingStun = true;
        }
    }
}

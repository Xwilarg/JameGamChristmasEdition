using JameGam;
using JameGam.Player;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class AIController : ACharacter
    {
        private bool _canMove;
        private bool _isStunned;

        private bool _isDead;

        private void Awake()
        {
            AwakeParent();
        }

        public override void ResetC()
        { }

        public override void SetStun(Vector2 dir)
        {
            StartCoroutine(SetStunEnumerator(dir));
        }
        private IEnumerator SetStunEnumerator(Vector2 dir)
        {
            _canMove = false;
            _anim.SetBool("IsStunned", true);
            _isStunned = true;
            _rb.velocity = dir;

            yield return new WaitForSeconds(1f);

            _canMove = true;
            _anim.SetBool("IsStunned", true);
            _isStunned = false;
            _rb.velocity = Vector2.zero;
        }

        public override void SetDeathStatus(bool value)
        {
            _isDead = true;
            _anim.SetBool("IsDead", true);
        }

        public override int NetworkID => -1;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Trap"))
            {
                SetDeathStatus(true);
            }
        }
    }
}

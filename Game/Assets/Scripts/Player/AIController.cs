using JameGam;
using JameGam.Player;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class AIController : ACharacter
    {
        private bool _canMove;
        private bool _isStunned;

        private bool _isDead;

        private NextNode _target;

        private void Awake()
        {
            AwakeParent();

            GameManager.Instance.SoloAI++;

            _target = GameObject.FindGameObjectsWithTag("Node").OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).First().GetComponent<NextNode>();
        }

        private void Update()
        {
            UpdateParent();
        }

        private void FixedUpdate()
        {
            _rb.velocity = (_target.transform.position - transform.position).normalized / 2f;

            _anim.SetFloat("X", _rb.velocity.x);
            _anim.SetFloat("Y", _rb.velocity.y);

            if (Vector2.Distance(_target.transform.position, transform.position) < .1f)
            {
                var targets = _target.NextNodes.Where(x => x.gameObject.GetInstanceID() != gameObject.GetInstanceID()).ToArray();
                _target = targets[Random.Range(0, targets.Length)];
            }
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
            GameManager.Instance.SoloAI--;
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

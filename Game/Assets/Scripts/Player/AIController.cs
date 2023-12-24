using JameGam;
using JameGam.Player;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class AIController : ACharacter
    {
        private bool _canMove = true;
        private bool _isStunned;

        private bool _isDead;

        private NextNode _target;
        private NextNode _last;

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
            if (!_canMove || _isStunned) return;

            if (_isDead)
            {
                if (Vector2.Distance(PlayerController.Instance.transform.position, transform.position) < .5f)
                {
                    _rb.velocity = Vector2.zero;
                }
                else
                {
                    _rb.velocity = (PlayerController.Instance.transform.position - transform.position).normalized / 2f;
                }
            }
            else
            {
                if (GameManager.Instance.DidAIDie)
                {
                    _rb.velocity = 2f * (_target.transform.position - transform.position).normalized / 3f;
                }
                else
                {
                    _rb.velocity = (_target.transform.position - transform.position).normalized / 2f;
                }

                if (Vector2.Distance(_target.transform.position, transform.position) < .1f)
                {
                    if (GameManager.Instance.DidAIDie)
                    {
                        _target = _target.NextNodes.OrderByDescending(x => Vector2.Distance(x.transform.position, PlayerController.Instance.transform.position)).First();
                    }
                    else
                    {
                        _last = _target;
                        var targets = _target.NextNodes.Where(x => x.gameObject.GetInstanceID() != _last.gameObject.GetInstanceID()).ToArray();
                        _target = targets[Random.Range(0, targets.Length)];
                    }
                }
            }

            var mov = _rb.velocity.normalized;
            _anim.SetFloat("X", mov.x);
            _anim.SetFloat("Y", mov.y);
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

            gameObject.layer = 0;

            GameManager.Instance.DidAIDie = true;
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

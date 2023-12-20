using System.Collections;
using UnityEngine;

namespace JameGam.Player
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private Animator _anim;

        private Vector2 _mov;
        private Vector2 _lastMov = Vector2.up;

        private bool _canMove = true;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
            _anim = GetComponent<Animator>();
        }

        private void Update()
        {
            var targetPos = transform.position.y;
            _sr.sortingOrder = (int)(-targetPos * 100f);
        }

        private void FixedUpdate()
        {
            Vector2 mov = _canMove ? _mov : Vector2.zero;

            _rb.velocity = mov;
            GameManager.Instance.SendSpacialInfo(transform.position, mov);
            if (_canMove)
            {
                if (mov.magnitude != 0f)
                {
                    if (Mathf.Abs(_mov.x) >= Mathf.Abs(_mov.y)) _lastMov = _mov.x < 0f ? Vector2.left : Vector2.right;
                    else _lastMov = _mov.y < 0f ? Vector2.down : Vector2.up;
                }

                _anim.SetFloat("X", mov.x);
                _anim.SetFloat("Y", mov.y);
            }
        }

        public void OnMove(Vector2 val)
        {
            _mov = val;
        }

        public IEnumerator OnAttack()
        {
            _anim.SetBool("IsAttacking", true);
            _canMove = false;
            _anim.SetFloat("X", _lastMov.x);
            _anim.SetFloat("Y", _lastMov.y);
            _rb.velocity = Vector2.zero;
            
            yield return new WaitForSeconds(.75f);
            
            _anim.SetBool("IsAttacking", false);
            _canMove = true;
        }
    }
}

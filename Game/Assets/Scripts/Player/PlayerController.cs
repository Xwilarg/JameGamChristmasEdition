using System.Collections;
using UnityEngine;

namespace JameGam.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hitVfx;

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
        private Animator _anim;

        private Vector2 _mov;
        private Vector2 _lastMov = Vector2.up;

        private bool _canMove = true;

        private CarryType _carry;

        public bool IsDead { private set; get; }

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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Trap"))
            {
                GameManager.Instance.SendDeath(null);
            }
        }

        public void OnMove(Vector2 val)
        {
            _mov = val;
        }

        private void SetAttackDir()
        {
            _canMove = false;
            _anim.SetFloat("X", _lastMov.x);
            _anim.SetFloat("Y", _lastMov.y);
            _anim.SetBool("IsAttacking", true);
            _rb.velocity = Vector2.zero;
        }

        public void Die()
        {
            IsDead = true;
            _anim.SetBool("IsDead", true);
        }

        public IEnumerator OnAttack()
        {
            if (_canMove && !IsDead)
            {
                var hitPos = (Vector2)transform.position + _lastMov * .2f;
                if (_carry == CarryType.None)
                {
                    SetAttackDir();

                    yield return new WaitForSeconds(.25f);

                    if (IsDead)
                    {
                        _anim.SetBool("IsAttacking", false);
                        _canMove = true;
                    }
                    else
                    {
                        Destroy(Instantiate(_hitVfx, hitPos, Quaternion.identity), .5f);

                        Collider2D[] res = new Collider2D[1];
                        if (Physics2D.OverlapCircleNonAlloc(hitPos, .2f, res, 1 << LayerMask.NameToLayer("Rock")) > 0)
                        {
                            Destroy(res[0].gameObject);
                            Destroy(Instantiate(_hitVfx, hitPos, Quaternion.identity), .5f);

                            _carry = CarryType.Rock;
                            _anim.SetInteger("Carrying", 1);
                            GameManager.Instance.UpdateObjectiveCraft();
                        }

                        yield return new WaitForSeconds(.5f);

                        _anim.SetBool("IsAttacking", false);
                        _canMove = true;
                    }
                }
                else if (_carry == CarryType.Rock)
                {
                    _rb.velocity = Vector2.zero;

                    Collider2D[] res = new Collider2D[1];
                    if (Physics2D.OverlapCircleNonAlloc(hitPos, .2f, res, 1 << LayerMask.NameToLayer("Anvil")) > 0)
                    {
                        SetAttackDir();
                        for (int i = 0; i < 3; i++)
                        {
                            Destroy(Instantiate(_hitVfx, hitPos, Quaternion.identity), .5f);
                        }
                        _carry = CarryType.Sword;
                        GameManager.Instance.UpdateObjectiveSword();
                        _anim.SetInteger("Carrying", 2);
                        yield return new WaitForSeconds(.5f);

                        _anim.SetBool("IsAttacking", false);
                        _canMove = true;
                    }
                }
                else if (_carry == CarryType.Sword)
                {
                    SetAttackDir();

                    yield return new WaitForSeconds(.25f);

                    if (IsDead)
                    {
                        _anim.SetBool("IsAttacking", false);
                        _canMove = true;
                    }
                    else
                    {
                        Collider2D[] res = new Collider2D[1];
                        if (Physics2D.OverlapCircleNonAlloc(hitPos, .2f, res, 1 << LayerMask.NameToLayer("Player")) > 0)
                        {
                            if (res[0].gameObject.GetInstanceID() == gameObject.GetInstanceID())
                            {
                                Debug.LogError("Player attempted to hit himself!");
                            }
                            else
                            {
                                var np = res[0].GetComponent<NetworkPlayer>();
                                np.SetDeathStatus(true);
                                GameManager.Instance.SendDeath(np.NetworkID); 
                            }
                        }

                        yield return new WaitForSeconds(.5f);

                        _anim.SetBool("IsAttacking", false);
                        _canMove = true;
                    }
                }
            }
        }
    }
}

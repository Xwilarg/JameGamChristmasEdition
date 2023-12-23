using Assets.Scripts.Player;
using System.Collections;
using UnityEngine;

namespace JameGam.Player
{
    public class PlayerController : ACharacter
    {
        [SerializeField]
        private GameObject _hitVfx;

        private Vector2 _mov;
        private Vector2 _lastMov = Vector2.up;

        private bool _canMove = true;

        private CarryType _carry;

        private bool _isDead;
        public bool IsDead
        {
            private set
            {
                _isDead = value;
                if (value)
                {
                    GameManager.Instance.UpdateObjectiveDead();
                }
            }
            get => _isDead;
        }

        private bool _needReset;
        private bool _isDirty;
        private bool _isStunned;

        private void Awake()
        {
            AwakeParent();
            IsDead = !GameManager.Instance.IsSolo;
        }

        private void Update()
        {
            UpdateParent();

            if (_isDirty)
            {
                _anim.SetBool("IsDead", IsDead);
            }

            if (_needReset)
            {
                _needReset = false;
                IsDead = false;
                _carry = CarryType.None;
                transform.position = Vector2.zero;
                _canMove = true;
                _anim.SetBool("IsAttacking", false);
                _anim.SetInteger("Carrying", 0);
            }
        }

        private void FixedUpdate()
        {
            Vector2 mov;

            if (_canMove) mov = _mov;
            else if (_isStunned) mov = _rb.velocity;
            else mov = Vector2.zero;

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
                SetDeathStatus(true);
                GameManager.Instance.SendDeath(null);
            }
        }

        public override void ResetC()
        {
            _needReset = true;
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
            GameManager.Instance.SendAttack();
        }

        public override void SetDeathStatus(bool value) // Must be true
        {
            IsDead = true;
            _isDirty = true;
        }

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
        }

        public IEnumerator OnAttack()
        {
            if (_canMove && !IsDead)
            {
                var hitPos = (Vector2)transform.position + _lastMov * .3f;
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
                            GameManager.Instance.SendCarry(CarryType.Rock);
                            _anim.SetInteger("Carrying", 1);
                            GameManager.Instance.UpdateObjectiveCraft();
                        }
                        if (Physics2D.OverlapCircleNonAlloc(hitPos, .2f, res, 1 << LayerMask.NameToLayer("Player")) > 0)
                        {
                            if (res[0].gameObject.GetInstanceID() == gameObject.GetInstanceID())
                            {
                                Debug.LogError("Player attempted to hit himself!");
                            }
                            else
                            {
                                var np = (ACharacter)res[0].GetComponent<NetworkPlayer>() ?? res[0].GetComponent<AIController>();
                                np.SetStun(_lastMov);
                                GameManager.Instance.SendStun(np.NetworkID, _lastMov);
                            }
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
                        GameManager.Instance.SendCarry(CarryType.Sword);
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
                                var np = (ACharacter)res[0].GetComponent<NetworkPlayer>() ?? res[0].GetComponent<AIController>();
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

        public override int NetworkID => -1;
    }
}

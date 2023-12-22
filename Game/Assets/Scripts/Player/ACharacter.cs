using UnityEngine;

namespace JameGam.Player
{
    public abstract class ACharacter : MonoBehaviour // Don't let Indra notice I put a A in front of my abstract class
    {

        protected Rigidbody2D _rb;
        protected SpriteRenderer _sr;
        protected Animator _anim;

        protected void AwakeParent()
        {
            _anim = GetComponent<Animator>();
            _sr = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
        }

        protected void UpdateParent()
        {
            var targetPos = transform.position.y;
            _sr.sortingOrder = (int)(-targetPos * 100f);
        }

        public abstract void ResetC();
    }
}

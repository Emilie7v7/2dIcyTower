using System;
using _Scripts.CoreSystem;
using _Scripts.ObjectPool.ObjectsToPool;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private float punchStrengthX;
        private Rigidbody2D _rb;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var rb = other.GetComponent<Rigidbody2D>();
                var stats = other.GetComponentInChildren<Stats>();
                
                if (!stats.IsImmortal)
                {
                    if (rb)
                    {
                        var punchDirection = (other.transform.position - transform.position).normalized;
                        
                        rb.velocity = Vector2.zero;
                        rb.AddForce(punchDirection * (punchStrengthX * 1.5f), ForceMode2D.Impulse);
                        rb.gravityScale = 0.5f;
                    }

                    if (stats)
                    {
                        stats.Health.DecreaseAmount(1);
                    }
                }
                
                ArrowPool.Instance?.ReturnObject(this);
            }

            if (other.CompareTag("Wall"))
            {
                ArrowPool.Instance?.ReturnObject(this);
            }

            if (other.CompareTag("SolidPlatform"))
            {
                ArrowPool.Instance?.ReturnObject(this);
            }
        }
        
        public void Launch(Vector2 direction, float speed)
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();

            _rb.gravityScale = 0f;
            _rb.velocity = direction.normalized * speed;

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.flipX = direction.x < 0;
        }
    }
}

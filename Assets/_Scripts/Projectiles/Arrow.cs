using System;
using _Scripts.CoreSystem;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private float speed;

        [SerializeField] private float punchStrengthX;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            
            _rb.velocity = transform.right * speed;

        }

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
                
                Destroy(gameObject);
            }

            if (other.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }

            if (other.CompareTag("SolidPlatform"))
            {
                Destroy(gameObject);
            }
        }
    }
}

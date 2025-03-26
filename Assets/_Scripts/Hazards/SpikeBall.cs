using System;
using _Scripts.CoreSystem;
using UnityEngine;

namespace _Scripts.Hazards
{
    public class SpikeBall : MonoBehaviour
    {

        [SerializeField] private float knockBackStrength;
        private bool _gotHit;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var rb = other.GetComponent<Rigidbody2D>();
                var stats = other.GetComponentInChildren<Stats>();

                if (!stats.IsImmortal)
                {
                    if(_gotHit) return;
                    
                    if (rb)
                    {
                        Vector2 direction = (other.transform.position - transform.position).normalized;

                        Vector2 knockBackDirection;
                        
                        var hitFromAbove = direction.y > 0.4f;

                        if (hitFromAbove)
                        {
                            knockBackDirection = new Vector2(Mathf.Sign(direction.x), 0f).normalized;
                        }
                        else
                        {
                            knockBackDirection = direction;
                        }

                        rb.velocity = Vector2.zero;
                        rb.AddForce(knockBackDirection * (knockBackStrength * 1.5f), ForceMode2D.Impulse);
                        rb.gravityScale = 0.5f;
                    }

                    if (stats)
                    {
                        _gotHit = true;
                        Debug.Log("gotHit");
                        stats.Health.DecreaseAmount(1);
                        stats.ActivateImmortality(2f);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _gotHit = false;
            }
        }
    }
}

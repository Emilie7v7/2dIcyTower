using System;
using _Scripts.CoreSystem;
using UnityEngine;

namespace _Scripts.Hazards
{
    public class Saw : MonoBehaviour
    {
        [SerializeField] private Transform[] targetPoints;
        [SerializeField] private float speed;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float knockBackStrength;
        
        private const int StartPoint = 0;
        private int i;

        private bool _gotHit = false;

        private void Start()
        {
            transform.position = targetPoints[StartPoint].position;
            _gotHit = false;
        }

        private void Update()
        {
            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            if (Vector2.Distance(transform.position, targetPoints[i].position) < 0.2f)
            {
                i++;
                if (i == targetPoints.Length)
                {
                    i = 0;
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, targetPoints[i].position, Time.deltaTime * speed);
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }

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


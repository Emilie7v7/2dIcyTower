using _Scripts.CoreSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Projectile
{
    public class Potion : CoreComponent
    {
        public GameObject projectile;
        [SerializeField] private float spawnOffset; // Offset for spawning the projectile
        [SerializeField] private float explosionForce = 10f;

        public CollisionSenses collisionSenses { get; private set; }// Reference to CollisionSenses
        public Core core { get; private set; } // Reference to Core
        public Movement movement { get; private set; }

        protected override void Awake()
        {
            core = GetComponentInChildren<Core>();
            collisionSenses = core.GetCoreComponent<CollisionSenses>();
            movement = core.GetCoreComponent<Movement>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Vector2 explosionPosition = transform.position;

            // Check if touching Enemy using CollisionSenses property
            if (collisionSenses.Enemy)
            {
                Destroy(gameObject);
                Explosion(explosionPosition);
                Debug.Log("Explosion caused by a detected Enemy.");
            }

            if (collisionSenses.Grounded)
            {
                Destroy(gameObject);
                Explosion(explosionPosition);
                Debug.Log("Explosion caused by a detected Ground.");
            }
        }

        private void ApplyExplosionForce(Vector2 explosionPosition)
        {
            // Find players within the explosion radius
            Collider2D[] playersInRadius = Physics2D.OverlapCircleAll(
                explosionPosition, 
                collisionSenses.EntityCheckRadius, 
                collisionSenses.WhatIsPlayer
            );

            // Find enemies within the explosion radius
            Collider2D[] enemiesInRadius = Physics2D.OverlapCircleAll(
                explosionPosition, 
                collisionSenses.EntityCheckRadius, 
                collisionSenses.WhatIsEnemy
            );
            
            

            // Apply explosion force to players
            foreach (Collider2D obj in playersInRadius)
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    Vector2 direction = (rb.position - explosionPosition).normalized;
                    rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                    Debug.Log($"Applied explosion force to Player: {obj.name}");
                }
            }
        
            // Apply explosion force to enemies
            foreach (Collider2D obj in enemiesInRadius)
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    Vector2 direction = (rb.position - explosionPosition).normalized;
                    rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                    Debug.Log($"Applied explosion force to Enemy: {obj.name}");
                }
            }
        }

        private void Explosion(Vector2 explosionPosition)
        {
            // Apply explosion logic
            ApplyExplosionForce(explosionPosition);
        }

        private void OnDrawGizmos()
        {
            if (core == null) return;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, collisionSenses.EntityCheckRadius);
        
        }
    }
}
using _Scripts.CoreSystem;
using UnityEngine;

namespace _Scripts.Projectile
{
    public class Potion : CoreComponent
    {
        [SerializeField] private GameObject projectile;
        [SerializeField] private float spawnOffset; // Offset for spawning the projectile
        [SerializeField] private float explosionForce = 10f;

        private CollisionSenses CollisionSenses { get; set; }// Reference to CollisionSenses
        private new Core Core { get; set; } // Reference to Core
        public Movement Movement { get; private set; }

        protected override void Awake()
        {
            Core = GetComponentInChildren<Core>();
            CollisionSenses = Core.GetCoreComponent<CollisionSenses>();
            Movement = Core.GetCoreComponent<Movement>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Vector2 explosionPosition = transform.position;

            // Check if touching Enemy using CollisionSenses property
            if (CollisionSenses.Enemy)
            {
                Destroy(gameObject);
                Explosion(explosionPosition);
                Debug.Log("Explosion caused by a detected Enemy.");
            }

            if (CollisionSenses.Grounded)
            {
                Destroy(gameObject);
                Explosion(explosionPosition);
                Debug.Log("Explosion caused by a detected Ground.");
            }
        }

        private void ApplyExplosionForce(Vector2 explosionPosition)
        {
            // Find players within the explosion radius
            var playersInRadius = Physics2D.OverlapCircleAll(
                explosionPosition, 
                CollisionSenses.EntityCheckRadius, 
                CollisionSenses.WhatIsPlayer
            );

            // Find enemies within the explosion radius
            var enemiesInRadius = Physics2D.OverlapCircleAll(
                explosionPosition, 
                CollisionSenses.EntityCheckRadius, 
                CollisionSenses.WhatIsEnemy
            );
            
            

            // Apply explosion force to players
            foreach (var obj in playersInRadius)
            {
                var rb = obj.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    var direction = (rb.position - explosionPosition).normalized;
                    rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
                    Debug.Log($"Applied explosion force to Player: {obj.name}");
                }
            }
        
            // Apply explosion force to enemies
            foreach (var obj in enemiesInRadius)
            {
                var rb = obj.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    var direction = (rb.position - explosionPosition).normalized;
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
            if (Core == null) return;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, CollisionSenses.EntityCheckRadius);
        }
    }
}
using System;
using _Scripts.Combat.Damage;
using _Scripts.CoreSystem;
using _Scripts.ScriptableObjects.ProjectileData;
using UnityEngine;

namespace _Scripts.Projectile
{
    public class Projectile : MonoBehaviour
    {
        private Vector2 _startPosition;
        private bool _hasHitGround;
        private Collider2D[] _results;
        
        [field: SerializeField] public Transform DamagePosition {get; private set;}
        [field: SerializeField] public ProjectileDataSo ProjectileData { get; private set; }
        public Core Core { get; private set; }
        public CollisionSenses CollisionSenses { get; private set; }
        public Movement Movement { get; private set; }

        private void Awake()
        {
            Core = GetComponentInChildren<Core>();
            CollisionSenses = Core.GetCoreComponent<CollisionSenses>();
            Movement = Core.GetCoreComponent<Movement>();

            _results = new Collider2D[CollisionSenses.MaxHitsRayForProjectile];
        }

        private void Start()
        {
            _hasHitGround = false;

            _startPosition = transform.position;
        }

        private void FixedUpdate()
        {
            var detectedCollisions = Physics2D.OverlapCircleNonAlloc(DamagePosition.position, ProjectileData.projectileRadius, _results, CollisionSenses.MultipleLayers);

            if (detectedCollisions > 0)
            {
                for (var i = 0; i < detectedCollisions; i++)
                {
                    var hit = _results[i];

                    if (hit)
                    {
                        Debug.Log($"Detected {detectedCollisions} projectiles");
                        
                        Destroy(gameObject);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(DamagePosition.position, ProjectileData.projectileRadius);
        }

        private void DetectedPlayerAndGround()
        {
            if (!_hasHitGround)
            {
                var playerHit = Physics2D.OverlapCircle(DamagePosition.position, ProjectileData.projectileRadius, CollisionSenses.WhatIsPlayer);
                var groundHit = Physics2D.OverlapCircle(DamagePosition.position, ProjectileData.projectileRadius, CollisionSenses.WhatIsGround);

                if (playerHit)
                {
                    var detectedObjects = Physics2D.OverlapCircleAll(DamagePosition.position, ProjectileData.projectileRadius, CollisionSenses.WhatIsPlayer);

                    // foreach (var colliders in detectedObjects)
                    // {
                    //     var damageable = colliders.GetComponent<IDamageable>();
                    //     
                    //     damageable?.Damage(new DamageData(ProjectileData.projectileDamage, Core.Root));
                    // }
                    foreach (var colliders in detectedObjects)
                    {
                        var damageable = colliders.GetComponent<IDamageable>();

                        if (damageable != null)
                        {
                            // Find the top-most parent (the Player)
                            var root = colliders.transform.root;

                            // Check if this is the actual Player (compare by tag or component)
                            if (root.CompareTag("Player"))
                            {
                                Debug.Log("Hit Player: " + root.name);
                                damageable.Damage(new DamageData(ProjectileData.projectileDamage, Core.Root));
                            }
                            else
                            {
                                Debug.Log("Hit something else: " + root.name);
                            }
                        }
                    }
                    Destroy(gameObject);
                }

                if (groundHit)
                {
                    _hasHitGround = true;
                    Movement.R2BD.velocity = Vector2.zero;
                    Movement.R2BD.gravityScale = 0f;
                    Destroy(gameObject);
                }
            }
        }
    }
}

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
        }

        private void Start()
        {
            _hasHitGround = false;

            _startPosition = transform.position;
        }

        private void Update()
        {
            if(_hasHitGround) return;
        }

        private void FixedUpdate()
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
                                Debug.Log("✅ Hit Player: " + root.name);
                                damageable.Damage(new DamageData(ProjectileData.projectileDamage, Core.Root));
                                Destroy(gameObject); // Destroy projectile after hitting player
                            }
                            else
                            {
                                Debug.Log("❌ Hit something else: " + root.name);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(DamagePosition.position, ProjectileData.projectileRadius);
        }
    }
}

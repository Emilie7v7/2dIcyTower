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
        public Rigidbody2D MyRigidbody2D { get; private set; }

        private void Awake()
        {
            Core = GetComponentInChildren<Core>();
            CollisionSenses = Core.GetCoreComponent<CollisionSenses>();
        }

        private void Start()
        {
            MyRigidbody2D = GetComponent<Rigidbody2D>();
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

                    foreach (var colliders in detectedObjects)
                    {
                        var damageable = colliders.GetComponent<IDamageable>();
                        
                        Debug.Log("HitPlayer");
                        damageable?.Damage(new DamageData(ProjectileData.projectileDamage, Core.Root));
                    }
                    Destroy(gameObject);
                }

                if (groundHit)
                {
                    _hasHitGround = true;
                    MyRigidbody2D.velocity = Vector2.zero;
                    MyRigidbody2D.gravityScale = 0f;
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

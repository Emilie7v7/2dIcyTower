using System;
using _Scripts.Combat.Damage;
using _Scripts.CoreSystem;
using _Scripts.ScriptableObjects.ProjectileData;
using UnityEngine;

namespace _Scripts.Projectile
{
    public class Projectile : MonoBehaviour
    {
        private bool _hasExploded;
        private Collider2D[] _results;

        [field: SerializeField] public Transform DetectionPosition { get; private set; }
        [field: SerializeField] public ProjectileDataSo ProjectileData { get; private set; }
        private Core _core;
        public Movement Movement { get; private set; }

        private void Awake()
        {
            _core = GetComponentInChildren<Core>();
            Movement = _core.GetCoreComponent<Movement>();

            _results = new Collider2D[ProjectileData.maxHitsRayForProjectile];
        }

        private void Start()
        {
            _hasExploded = false;
        }

        private void FixedUpdate()
        {
            ProjectileHitDetection();
        }

        private void ProjectileHitDetection()
        {
            if (_hasExploded) return; // Prevents multiple explosions

            var detectedCollisions = Physics2D.OverlapCircleNonAlloc(
                new Vector2(DetectionPosition.position.x, DetectionPosition.position.y),
                ProjectileData.projectileRadius,
                _results,
                ProjectileData.targetLayer
            );

            if (detectedCollisions > 0)
            {
                for (var i = 0; i < detectedCollisions; i++)
                {
                    var hit = _results[i];

                    if (hit)
                    {
                        _hasExploded = true; // Prevents multiple explosions

                        var hitPosition = hit.ClosestPoint(transform.position);
                        
                        Instantiate(ProjectileData.explosionPrefab, hitPosition, Quaternion.identity);
                        Destroy(gameObject);
                        break; // Stops loop after first valid collision
                    }
                }
            }
        }

    private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(DetectionPosition.position, ProjectileData.projectileRadius);
        }
    }
}

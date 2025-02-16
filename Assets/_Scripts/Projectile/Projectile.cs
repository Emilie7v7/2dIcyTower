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
        private Vector2 _spawnPosition;

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
            _spawnPosition = transform.position; // Save initial position
        }

        private void FixedUpdate()
        {
            //ProjectileRaycastCheck();
            ProjectileHitDetection();

            // Destroy projectile if it has traveled too far
            if (Vector2.Distance(_spawnPosition, transform.position) >= ProjectileData.maxTravelDistance)
            {
                Destroy(gameObject);
            }
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
        private void ProjectileRaycastCheck()
        {
            Vector2 position = transform.position;
            Vector2 velocity = Movement.CurrentVelocity.normalized;
            float distance = ProjectileData.projectileSpeed * Time.fixedDeltaTime; 

            RaycastHit2D hit = Physics2D.Raycast(position, velocity, distance, ProjectileData.targetLayer);

            if (hit.collider != null)
            {
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

                _hasExploded = true;
                Instantiate(ProjectileData.explosionPrefab, hit.point, Quaternion.identity);
                Destroy(gameObject);
            }
        }

    private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(DetectionPosition.position, ProjectileData.projectileRadius);
        }
    }
}

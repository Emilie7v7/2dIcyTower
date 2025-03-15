using _Scripts.CoreSystem;
using _Scripts.ObjectPool;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.ScriptableObjects.ProjectileData;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        private Collider2D[] _results;
        private Vector2 _spawnPosition;

        [field: SerializeField] public Transform DetectionPosition { get; private set; }
        [field: SerializeField] public ProjectileDataSo ProjectileData { get; private set; }
        
        private Core _core;
        public Movement Movement { get; private set; }

        private bool _isPlayerProjectile;

        private void Awake()
        {
            _core = GetComponentInChildren<Core>();
            Movement = _core.GetCoreComponent<Movement>();

            _results = new Collider2D[ProjectileData.maxHitsRayForProjectile];
        }

        public void SetProjectileOwner(bool isPlayer)
        {
            _isPlayerProjectile = isPlayer;
        }

        private void FixedUpdate()
        {
            ProjectileHitDetection();

            if (Vector2.Distance(_spawnPosition, transform.position) >= ProjectileData.maxTravelDistance)
            {
                ReturnToPool();
            }
        }

        private void ProjectileHitDetection()
        {
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
                        var hitPosition = hit.ClosestPoint(transform.position);
                        
                        // Get explosion from the correct pool
                        var explosion = _isPlayerProjectile
                            ? PlayerExplosionPool.Instance?.GetObject(hitPosition)
                            : EnemyExplosionPool.Instance?.GetObject(hitPosition);

                        if (explosion != null)
                        {
                            explosion.ActivateExplosion(hitPosition, _isPlayerProjectile); //Trigger explosion properly
                        }

                        ReturnToPool(); // Ensure projectile is returned properly
                        break;
                    }
                }
            }
        }
        
        public void SetSpawnPosition(Vector3 position)
        {
            _spawnPosition = position; // Updates spawn point to new position
        }

        private void ReturnToPool()
        {
            if (_isPlayerProjectile)
            {
                PlayerProjectilePool.Instance.ReturnObject(this);
            }
            else
            {
                EnemyProjectilePool.Instance.ReturnObject(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(DetectionPosition.position, ProjectileData.projectileRadius);
        }
    }
}
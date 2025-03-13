using System.Collections;
using _Scripts.Combat.Damage;
using _Scripts.CoreSystem;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.ScriptableObjects.ExplosionData;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private ExplosionDataSo explosionDataSo;
        [SerializeField] private bool isPlayerExplosion = false;
        [SerializeField] private ParticleSystem explosionParticles; // Reference to the Particle System
        [field: SerializeField] public Transform DetectionPosition { get; private set; }
        private Collider2D[] _colliders;

        private bool _hasExploded;
        private Core _core;
        
        private void Awake()
        {
            _core = GetComponentInChildren<Core>();

            _colliders = new Collider2D[explosionDataSo.maxHitsRayForExplosion];
            _hasExploded = false;
        }

        public void ActivateExplosion(Vector3 position, bool isPlayer)
        {
            isPlayerExplosion = isPlayer;
            transform.position = position;

            //Debug.Log("Explosion Activated at: " + position);

            Explode(); //Manually trigger explosion effect every time it's retrieved
        }
        private void Explode()
        {
            if(_hasExploded) return;
            _hasExploded = true;
            // Play the explosion particle effect manually
            if (explosionParticles is not null)
            {
                explosionParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); //Ensure old particles are cleared
                explosionParticles.Play(); //Start explosion effect again
            }
            
            // Calculate effective explosion radius
            var effectiveRadius = explosionDataSo.explosionRadius;
            if (isPlayerExplosion && GameManager.Instance?.PlayerData != null)
            {
                effectiveRadius += GameManager.Instance.PlayerData.explosionRadiusBonus;
                //Debug.Log("Player Explosion Radius is: " + effectiveRadius);
            }

            #region Damage Collision

            // Detect colliders for damage application
            var detectedColliders = Physics2D.OverlapCircleNonAlloc(
                DetectionPosition.position, 
                effectiveRadius, 
                _colliders, 
                explosionDataSo.targetLayer);

            if (detectedColliders > 0)
            {
                for (var i = 0; i < detectedColliders; i++)
                {
                    var hit = _colliders[i];

                    if (hit && hit.gameObject.CompareTag("Player"))
                    {
                        var stats = hit.GetComponentInChildren<Stats>();
                        if (stats is not null && !stats.IsImmortal)
                        {
                            stats.Health.DecreaseAmount(explosionDataSo.explosionDamage);
                        }
                    }

                    if (hit && hit.gameObject.CompareTag("Enemy"))
                    {
                        var damageable = hit.GetComponentInChildren<IDamageable>();
                        damageable?.Damage(new DamageData(explosionDataSo.explosionDamage, _core.Root));
                    }
                }
            }

            #endregion

            #region No Damage Collision

            // Detect colliders for no-damage effects (e.g., applying impulse force)
            var noDamageDetectedColliders = Physics2D.OverlapCircleNonAlloc(
                DetectionPosition.position,
                effectiveRadius,
                _colliders,
                explosionDataSo.noDamageLayer);

            if (noDamageDetectedColliders > 0)
            {
                for (var i = 0; i < noDamageDetectedColliders; i++)
                {
                    var hit = _colliders[i];

                    if (hit && hit.gameObject.CompareTag("Player"))
                    {
                        var rb = hit.GetComponent<Rigidbody2D>(); 
                        if (rb is not null)
                        {
                            var explosionDirection = (hit.transform.position - DetectionPosition.position).normalized;
                            var distance = Vector2.Distance(hit.transform.position, DetectionPosition.position);

                            // Scale force magnitude based on distance relative to effectiveRadius
                            var forceMagnitude = Mathf.Lerp(explosionDataSo.explosionStrength.y, explosionDataSo.explosionStrength.x,
                                distance / effectiveRadius) * 1.8f;

                            rb.velocity = Vector2.zero;
                            rb.AddForce(explosionDirection * (forceMagnitude * 1.5f), ForceMode2D.Impulse);
                            rb.gravityScale = 0.5f;
                        }
                    }
                }
            }

            #endregion
            

            // Start coroutine to return to pool after particle system finishes
            StartCoroutine(WaitForParticlesToEnd());
        }

        
        private IEnumerator WaitForParticlesToEnd()
        {
            if (explosionParticles is not null)
            {
                yield return new WaitUntil(() => !explosionParticles.IsAlive(true)); // Wait for the effect to finish
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (isPlayerExplosion)
            {
                _hasExploded = false;
                PlayerExplosionPool.Instance.ReturnObject(this);
            }
            else
            {
                _hasExploded = false;
                EnemyExplosionPool.Instance.ReturnObject(this);
            }
        }
        private void OnDrawGizmos()
        {
            // Draw the effective explosion radius for debugging
            var effectiveRadius = explosionDataSo.explosionRadius;
            if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
            {
                effectiveRadius += GameManager.Instance.PlayerData.explosionRadiusBonus;
            }
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(DetectionPosition.position, effectiveRadius);
        }
    }
}

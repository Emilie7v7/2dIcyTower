using System;
using System.Collections.Generic;
using _Scripts.Combat.Damage;
using _Scripts.CoreSystem;
using _Scripts.ScriptableObjects.ExplosionData;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Projectile
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private ExplosionDataSo explosionDataSo;
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

        private void Start()
        {
            Explode();
        }

        private void Explode()
        {
            if (_hasExploded) return;

            _hasExploded = true; // Ensures explosion only happens once

            var detectedColliders = Physics2D.OverlapCircleNonAlloc(
                new Vector2(DetectionPosition.position.x, DetectionPosition.position.y), 
                explosionDataSo.explosionRadius, _colliders,explosionDataSo.targetLayer);
            
            
            if (detectedColliders > 0)
            {
                for (var i = 0; i < detectedColliders; i++)
                {
                    var hit = _colliders[i];

                    Debug.Log($"Collider Detected: {hit.gameObject.name}", hit.gameObject);

                    if (hit && hit.gameObject.CompareTag("Player"))
                    {
                        var damageable = hit.GetComponentInChildren<IDamageable>();
                        damageable?.Damage(new DamageData(explosionDataSo.explosionDamage, _core.Root));
                        break;
                    }

                    if (hit && hit.gameObject.CompareTag("Enemy"))
                    {
                        var damageable = hit.GetComponentInChildren<IDamageable>();
                        Debug.Log($"{hit.gameObject.name} damaged");
                        damageable?.Damage(new DamageData(explosionDataSo.explosionDamage, _core.Root));
                    }
                }
            }

            var noDamageDetectedColliders = Physics2D.OverlapCircleNonAlloc(
                new Vector2(DetectionPosition.position.x, DetectionPosition.position.y),
                explosionDataSo.explosionRadius, _colliders, explosionDataSo.noDamageLayer);
            
            if (noDamageDetectedColliders > 0)
            {
                for (var i = 0; i < noDamageDetectedColliders; i++)
                {
                    var hit = _colliders[i];
                    
                    if (hit && hit.gameObject.CompareTag("Player"))
                    {
                        Debug.Log($"Player hit {hit.gameObject.name}");
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(DetectionPosition.position, explosionDataSo.explosionRadius);
        }
    }
}

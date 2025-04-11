using System;
using System.Collections;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.Hazards
{
    public class DartTrap : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private ParticleSystem trapParticles;
        [SerializeField] private float dartCooldown;
        [SerializeField] private float arrowSpeed = 10f;
        [SerializeField] private Vector2 shootDirection;
        
        private void OnEnable()
        {
            StartCoroutine(LaunchDart());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator LaunchDart()
        {
            while (true)
            {
                trapParticles.Play();
                var arrowPrefab = ArrowPool.Instance?.GetObject(spawnPoint.position, Quaternion.identity);
                if (arrowPrefab != null)
                {
                    var arrow = arrowPrefab.GetComponent<Arrow>();
                    arrow.Launch(shootDirection, arrowSpeed);
                }
                yield return new WaitForSeconds(dartCooldown);
            }
        }
    }
}

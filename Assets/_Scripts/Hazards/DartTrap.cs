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
        
        private bool shouldShoot;
        private Coroutine shootingCoroutine;

        private void OnEnable()
        {
            // Start shooting when the chunk becomes active
            StartShooting();
        }

        private void OnDisable()
        {
            // Stop shooting when the chunk becomes inactive
            StopShooting();
        }

        private void StartShooting()
        {
            shouldShoot = true;
            if (shootingCoroutine == null)
            {
                shootingCoroutine = StartCoroutine(LaunchDart());
            }
        }

        private void StopShooting()
        {
            shouldShoot = false;
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
        }

        private IEnumerator LaunchDart()
        {
            while (shouldShoot)
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
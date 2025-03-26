using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.Hazards
{
    public class DartTrap : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject dart;
        [SerializeField] private ParticleSystem trapParticles;
        [SerializeField] private float dartCooldown;
        
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
                Instantiate(dart, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(dartCooldown);
            }
        }
    }
}

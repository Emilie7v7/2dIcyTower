using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class ParticleManager : CoreComponent
    {
        private Transform _particleManager;

        protected override void Awake()
        {
            base.Awake();
    
            var particleManagerObj = GameObject.FindGameObjectWithTag("ParticleManager");
            if (particleManagerObj is null)
            {
                return;
            }
            _particleManager = particleManagerObj.transform;
        }

        private static GameObject StartParticles(GameObject particlePrefab, Vector2 position, Quaternion rotation)
        {
            var particleInstance = Instantiate(particlePrefab, position, rotation);
            particleInstance.transform.parent = null;
            return particleInstance;
        }

        public GameObject StartParticles(GameObject particlePrefab)
        {
            return StartParticles(particlePrefab, transform.position, Quaternion.identity);
        }

        public GameObject StartParticlesWithRandomRotation(GameObject particlePrefab)
        {
            var randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            return StartParticles(particlePrefab, transform.position, randomRotation);
        }
    }
}

using System;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class Death : CoreComponent
    {
        [SerializeField] private GameObject[] deathParticles;
        
        private ParticleManager _particleManager;
        private Stats _stats;

        protected override void Awake()
        {
            base.Awake();

            _particleManager = Core.GetCoreComponent<ParticleManager>();
            _stats = Core.GetCoreComponent<Stats>();
        }

        private void Die()
        {
            foreach (var particle in deathParticles)
            {
                _particleManager.StartParticles(particle);
            }
            
            Core.transform.parent.gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            _stats.Health.OnCurrentValueZero += Die;
        }

        private void OnDisable()
        {
            _stats.Health.OnCurrentValueZero -= Die;
        }
    }
}

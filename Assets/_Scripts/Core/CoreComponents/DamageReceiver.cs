using _Scripts.Combat.Damage;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class DamageReceiver : CoreComponent, IDamageable
    {
        [SerializeField] private GameObject damageParticle;
        
        private ParticleManager _particleManager;


        protected override void Awake()
        {
            base.Awake();

            _particleManager = Core.GetCoreComponent<ParticleManager>();
        }

        public void Damage(DamageData data)
        {
            _particleManager.StartParticlesWithRandomRotation(damageParticle);
        }
    }
}

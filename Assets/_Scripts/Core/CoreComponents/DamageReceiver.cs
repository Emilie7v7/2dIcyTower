using _Scripts.Combat.Damage;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class DamageReceiver : CoreComponent, IDamageable
    {
        [SerializeField] private GameObject damageParticle;
        
        private ParticleManager _particleManager;
        private Stats _stats;
        
        protected override void Awake()
        {
            base.Awake();

            _particleManager = Core.GetCoreComponent<ParticleManager>();
            _stats = Core.GetCoreComponent<Stats>();
        }

        public void Damage(DamageData data)
        {
            //Debug.Log(Core.transform.parent.name + " Damage Dealt: " + data.Amount);
            _stats.Health.DecreaseAmount(data.Amount);
            _particleManager.StartParticlesWithRandomRotation(damageParticle);
            //Debug.Log(_stats.Health.CurrentValue + " / " + _stats.Health.MaxValue);
        }
    }
}

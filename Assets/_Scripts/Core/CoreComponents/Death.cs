using _Scripts.Entities.EntityStateMachine;
using _Scripts.Managers.Drop_Items_Logic;
using _Scripts.ObjectPool.ObjectsToPool;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class Death : CoreComponent
    {
        [SerializeField] private GameObject[] deathParticles;
        
        private ParticleManager _particleManager;
        private Stats _stats;
        private bool _isPlayer;

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

            _isPlayer = gameObject.CompareTag("Player");
            if (_isPlayer)
            {
                Core.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                var enemy = Core.GetComponentInParent<Entity>(); //Ensure you get the correct Entity component
                if (enemy is not null)
                {
                    DropManager.Instance.SpawnDroppedItem(transform.position);
                    EnemyPool.Instance.ReturnObject(enemy); //Return the whole enemy entity
                }
                else
                {
                    Debug.LogError("⚠ Enemy entity not found when trying to return to pool!");
                }
            }
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

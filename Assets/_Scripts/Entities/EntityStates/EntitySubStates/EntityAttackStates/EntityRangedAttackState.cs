using _Scripts.Entities.EntityStateMachine;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.Projectiles;
using _Scripts.ScriptableObjects.EntityData;
using _Scripts.ScriptableObjects.ProjectileData;
using UnityEngine;

namespace _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates
{
    public class EntityRangedAttackState : EntityAttackState
    {
        private GameObject _player;
        protected EntityRangedAttackState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }
        
        public override void AnimationAttackTrigger()
        {
            base.AnimationAttackTrigger();

            _player = GameObject.FindGameObjectWithTag("Player");
            
            if (_player is null)
            {
                //Debug.Log("Player is null");
                return;
            }
            
            var projectilePrefab = EnemyProjectilePool.Instance.GetObject(Entity.transform.position);
            if (projectilePrefab is null)
            {
                Debug.Log("Something is wrong with the projectile pool");
                return;
            }
            
            projectilePrefab.SetProjectileOwner(false);
            projectilePrefab.transform.position = Entity.transform.position;
            
            Vector2 direction = (_player.transform.position - Entity.transform.position).normalized;

            direction += projectilePrefab.ProjectileData.projectileArc;

            projectilePrefab.Movement.LaunchProjectile(direction, projectilePrefab.ProjectileData.projectileSpeed);
        }       
    }
}

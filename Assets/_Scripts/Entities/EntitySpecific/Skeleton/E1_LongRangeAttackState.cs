using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntitySpecific.Skeleton
{
    public class E1LongRangeAttackState : EntityRangedAttackState
    {
        private readonly Entity1 _enemy;
        private GameObject _player;
    
        public E1LongRangeAttackState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName, Entity1 enemy) : base(entity, stateMachine, entityData, animBoolName)
        {
            _enemy = enemy;
        }

        public override void Enter()
        {
            base.Enter();
        
            Movement?.SetZeroVelocity();
            //Debug.Log("We have entered Long Range State");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if(!IsAnimationFinished) return;

            if (IsPlayerInLineOfSight)
            {
                StateMachine.ChangeState(_enemy.PlayerDetectedState);
            }
            else
            {
                StateMachine.ChangeState(_enemy.IdleState);
            }
        }
        
        public override void AnimationAttackTrigger()
        {
            base.AnimationAttackTrigger();

            _player = GameObject.FindGameObjectWithTag("Player");
            
            if (_player is null)
            {
                return;
            }
            
            var projectilePrefab = EnemyProjectilePool.Instance.GetObject(Entity.transform.position);
            if (projectilePrefab is null)
            {
                Debug.Log("Something is wrong with the projectile pool");
                return;
            }
            
            // In the AnimationAttackTrigger method, before using shootPoint
            if (!_enemy.shootPoint)
            {
                var newShootPoint = new GameObject("ShootPoint");
                newShootPoint.transform.position = _enemy.transform.position + _enemy.transform.right * 2f;
                newShootPoint.transform.parent = _enemy.transform;
                _enemy.shootPoint = newShootPoint.transform;
            }

            projectilePrefab.SetProjectileOwner(false);
            projectilePrefab.transform.position = _enemy.shootPoint.transform.position;
            
            Vector2 direction = (_player.transform.position - _enemy.shootPoint.transform.position).normalized;

            direction += projectilePrefab.ProjectileData.projectileArc;

            projectilePrefab.Movement.LaunchProjectile(direction, projectilePrefab.ProjectileData.projectileSpeed);
        }
    }
}

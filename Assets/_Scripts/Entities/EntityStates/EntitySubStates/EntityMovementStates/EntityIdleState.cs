using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySuperState;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntityStates.EntitySubStates.EntityMovementStates
{
    public class EntityIdleState : EntityGroundState
    {
        private float _idleTime;

        protected bool IsIdleTimeOver;
        private bool _setFlipAfterIdleBool;
        
        public EntityIdleState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        
            IsIdleTimeOver = false;
            
            Movement?.SetZeroVelocity();
            SetRandomIdleTime();
        }

        public override void Exit()
        {
            base.Exit();

            if (_setFlipAfterIdleBool)
            {
                Movement?.Flip();
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (!Movement.CanSetVelocity)
            {
                Movement?.SetZeroVelocity();
            }
            else
            {
                Movement?.SetVelocityY(Movement.Rb2D.velocity.y);
            }

            if (Time.time >= StartTime + _idleTime)
            {
                IsIdleTimeOver = true;
            }
        }

        private void SetRandomIdleTime()
        {
            _idleTime = Random.Range(EntityData.minIdleTime, EntityData.maxIdleTime);
        }

        public void SetFlipAfterIdle(bool flipAfterIdle)
        {
            _setFlipAfterIdleBool = flipAfterIdle;
        }
    }
}

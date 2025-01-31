using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySuperState;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntityStates.EntitySubStates
{
    public class EntityIdleState : EntityGroundState
    {
        protected float IdleTime;

        protected bool IsIdleTimeOver;
        protected bool SetFlipAfterIdleBool;
        
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

            if (SetFlipAfterIdleBool)
            {
                Movement?.Flip();
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            Movement?.SetVelocityY(Movement.R2BD.velocity.y);

            if (Time.time >= StartTime + IdleTime)
            {
                IsIdleTimeOver = true;
            }
        }

        private void SetRandomIdleTime()
        {
            IdleTime = Random.Range(EntityData.minIdleTime, EntityData.maxIdleTime);
        }

        public void SetFlipAfterIdle(bool flipAfterIdle)
        {
            SetFlipAfterIdleBool = flipAfterIdle;
        }
    }
}

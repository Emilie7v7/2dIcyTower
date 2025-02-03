using _Scripts.CoreSystem;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntityStateMachine
{
    public abstract class EntityState
    {

        protected Core Core;
    
        protected readonly Entity Entity;
        protected readonly EntityStateMachine StateMachine;
        protected readonly EntityDataSo EntityData;

        protected float StartTime;
        
        protected bool IsAnimationFinished;
        protected bool IsExitingState;
        
        private string _animationBoolName;
        
        protected EntityState(Entity entity, EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName)
        {
            Entity = entity;
            StateMachine = stateMachine;
            EntityData = entityData;
            _animationBoolName = animBoolName;
            Core = entity.Core;
        }
        
        public virtual void Enter()
        {
            DoChecks();
            Entity.MyAnimator.SetBool(_animationBoolName, true);
            StartTime = Time.time;
            IsExitingState = false;
            IsAnimationFinished = false;
        }

        public virtual void Exit()
        {
            Entity.MyAnimator.SetBool(_animationBoolName, false);
            IsExitingState = true;
        }
        
        public virtual void LogicUpdate() {}
        
        public virtual void PhysicsUpdate() { DoChecks(); }

        public virtual void DoChecks() {}

        public virtual void AnimationTrigger() {}
        public virtual void AnimationFinishedTrigger() => IsAnimationFinished = true;
        public virtual void AnimationAttackTrigger() {}
    }
}

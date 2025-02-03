using _Scripts.CoreSystem;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;

namespace _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates
{
    public class EntityAttackState : EntityState
    {
        
        protected bool IsPlayerInLineOfSight;
        
        protected Movement Movement => _movement ? _movement : _movement = Core.GetCoreComponent<Movement>();
        private Movement _movement;

        private CollisionSenses CollisionSenses => _collisionSenses ? _collisionSenses : _collisionSenses = Core.GetCoreComponent<CollisionSenses>();
        private CollisionSenses _collisionSenses;

        protected EntityAttackState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }

        public override void DoChecks()
        {
            base.DoChecks();

            if (CollisionSenses)
            {
                IsPlayerInLineOfSight = CollisionSenses.IsPlayerInLineOfSight();
            }
        }

        public override void Enter()
        {
            base.Enter();

            Entity.AnimationToStateMachine.EntityAttackState = this;
            IsAnimationFinished = false;
            
            Movement?.SetZeroVelocity();
        }
    }
}

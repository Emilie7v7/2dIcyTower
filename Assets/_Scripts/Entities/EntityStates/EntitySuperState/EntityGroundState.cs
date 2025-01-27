using _Scripts.CoreSystem;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;

namespace _Scripts.Entities.EntityStates.EntitySuperState
{
    public class EntityGroundState : EntityState
    {
        //Checks
        protected bool IsGrounded;
    
        public EntityGroundState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }

        protected Movement Movement {get => movement != null ? movement : Core.GetCoreComponent<Movement>();}
        private Movement movement;
    
        protected CollisionSenses CollisionSenses {get => collisionSenses != null ? collisionSenses : Core.GetCoreComponent<CollisionSenses>();}
        private CollisionSenses collisionSenses;
    
        public override void DoChecks()
        {
            base.DoChecks();

            IsGrounded = CollisionSenses.Ground;
        }
    }
}

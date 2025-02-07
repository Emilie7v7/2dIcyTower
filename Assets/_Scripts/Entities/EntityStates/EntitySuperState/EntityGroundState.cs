using _Scripts.CoreSystem;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;

namespace _Scripts.Entities.EntityStates.EntitySuperState
{
    public class EntityGroundState : EntityState
    {
        //Checks
        protected bool IsGrounded;
        protected bool IsPlayerInLineOfSight;
        protected bool IsDetectingLedge;

        protected EntityGroundState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }

        protected Movement Movement => _movement ? _movement : _movement = Core.GetCoreComponent<Movement>();
        private Movement _movement;

        private CollisionSenses CollisionSenses => _collisionSenses ? _collisionSenses : _collisionSenses = Core.GetCoreComponent<CollisionSenses>();
        private CollisionSenses _collisionSenses;
    
        public override void DoChecks()
        {
            base.DoChecks();

            if (CollisionSenses)
            {
                IsGrounded = CollisionSenses.Grounded;
                IsPlayerInLineOfSight = CollisionSenses.IsPlayerInLineOfSight();
                IsDetectingLedge = CollisionSenses.LedgeVertical;
            }
        }
    }
}

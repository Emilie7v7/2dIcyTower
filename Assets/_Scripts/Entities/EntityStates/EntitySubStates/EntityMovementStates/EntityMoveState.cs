using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySuperState;
using _Scripts.ScriptableObjects.EntityData;

namespace _Scripts.Entities.EntityStates.EntitySubStates.EntityMovementStates
{
    public class EntityMoveState : EntityGroundState
    {
        protected EntityMoveState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            Movement?.SetVelocityX(EntityData.movementSpeed * Movement.FacingDirection);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (Movement.CurrentVelocity.y <= 0.01f && !Movement.CanSetVelocity)
            {
                Movement.SetZeroVelocity();
            }
            else
            {
                Movement?.SetVelocityX(EntityData.movementSpeed * Movement.FacingDirection);
            }
        }
    }
}

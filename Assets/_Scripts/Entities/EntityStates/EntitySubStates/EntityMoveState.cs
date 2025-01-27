using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySuperState;
using _Scripts.ScriptableObjects.EntityData;

namespace _Scripts.Entities.EntityStates.EntitySubStates
{
    public class EntityMoveState : EntityGroundState
    {
        public EntityMoveState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            Movement?.SetVelocityX(EntityData.MovementSpeed * Movement.FacingDirection);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (Movement.CanSetVelocity)
            {
                Movement?.SetVelocityX(EntityData.MovementSpeed * Movement.FacingDirection);
            }
        }
    }
}

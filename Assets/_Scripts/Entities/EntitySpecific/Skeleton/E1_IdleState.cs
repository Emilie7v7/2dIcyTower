using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySubStates.EntityMovementStates;
using _Scripts.ScriptableObjects.EntityData;

namespace _Scripts.Entities.EntitySpecific.Skeleton
{
    public class E1IdleState : EntityIdleState
    {
        private Entity1 _enemy;
        
        public E1IdleState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName, Entity1 enemy) : base(entity, stateMachine, entityData, animBoolName)
        {
            this._enemy = enemy;
        }
        
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (IsPlayerInLineOfSight)
            {
                StateMachine.ChangeState(_enemy.PlayerDetectedState);
            }
            
            if (IsIdleTimeOver)
            {
                StateMachine.ChangeState(_enemy.MoveState);
            }
        }
    }
}

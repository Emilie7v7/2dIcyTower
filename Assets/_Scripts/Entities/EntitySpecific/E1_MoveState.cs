using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;
using EntityMoveState = _Scripts.Entities.EntityStates.EntitySubStates.EntityMovementStates.EntityMoveState;

namespace _Scripts.Entities.EntitySpecific
{
    public class E1MoveState : EntityMoveState
    {
        private Entity1 _enemy;
        
        public E1MoveState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName, Entity1 enemy) : base(entity, stateMachine, entityData, animBoolName)
        {
            this._enemy = enemy;
        }

        public override void Enter()
        {
            base.Enter();
            
            //Debug.Log("We have entered the MoveState");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (IsPlayerInLineOfSight)
            {
                StateMachine.ChangeState(_enemy.PlayerDetectedState);
            }
            else if (!IsDetectingLedge)
            {
                _enemy.IdleState.SetFlipAfterIdle(true);
                StateMachine.ChangeState(_enemy.IdleState);
            }
        }
    }
}

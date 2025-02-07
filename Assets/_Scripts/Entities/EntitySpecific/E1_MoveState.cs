using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySubStates;
using _Scripts.Entities.EntityStates.EntitySubStates.EntityMovementStates;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;
using EntityMoveState = _Scripts.Entities.EntityStates.EntitySubStates.EntityMovementStates.EntityMoveState;

namespace _Scripts.Entities.EntitySpecific
{
    public class E1_MoveState : EntityMoveState
    {
        private Entity1 enemy;
        
        public E1_MoveState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName, Entity1 enemy) : base(entity, stateMachine, entityData, animBoolName)
        {
            this.enemy = enemy;
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
                StateMachine.ChangeState(enemy.PlayerDetectedState);
            }
            else if (!IsDetectingLedge)
            {
                enemy.IdleState.SetFlipAfterIdle(true);
                StateMachine.ChangeState(enemy.IdleState);
            }
        }
    }
}

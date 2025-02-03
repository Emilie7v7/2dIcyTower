using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySubStates;
using _Scripts.Entities.EntityStates.EntitySubStates.EntityMovementStates;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntitySpecific
{
    public class E1_IdleState : EntityIdleState
    {
        private Entity1 enemy;
        
        public E1_IdleState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName, Entity1 enemy) : base(entity, stateMachine, entityData, animBoolName)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            base.Enter();
            
            Debug.Log("We have entered the IdleState");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (IsPlayerInLineOfSight)
            {
                StateMachine.ChangeState(enemy.PlayerDetectedState);
            }
            
            if (IsIdleTimeOver)
            {
                StateMachine.ChangeState(enemy.MoveState);
            }
        }
    }
}

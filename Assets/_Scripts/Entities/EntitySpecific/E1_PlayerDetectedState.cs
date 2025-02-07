using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntitySpecific
{
    public class E1_PlayerDetectedState : EntityPlayerDetectedState
    {
        private Entity1 enemy;
    
        public E1_PlayerDetectedState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName, Entity1 enemy) : base(entity, stateMachine, entityData, animBoolName)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            base.Enter();
            
            //Debug.Log("We have entered the player detected state");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (PerformLongRangeAction)
            {
                StateMachine.ChangeState(enemy.LongRangeAttackState);
            }
            else if (!IsPlayerInLineOfSight)
            {
                StateMachine.ChangeState(enemy.IdleState);
            }
        }
    }
}

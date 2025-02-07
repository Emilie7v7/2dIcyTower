using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntitySpecific
{
    public class E1_LongRangeAttackState : EntityRangedAttackState
    {
        private Entity1 enemy;
    
        public E1_LongRangeAttackState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName, Entity1 enemy) : base(entity, stateMachine, entityData, animBoolName)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            base.Enter();
        
            Movement?.SetZeroVelocity();
            //Debug.Log("We have entered Long Range State");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if(!IsAnimationFinished) return;

            if (IsPlayerInLineOfSight)
            {
                StateMachine.ChangeState(enemy.PlayerDetectedState);
            }
            else
            {
                StateMachine.ChangeState(enemy.IdleState);
            }
        }
    }
}

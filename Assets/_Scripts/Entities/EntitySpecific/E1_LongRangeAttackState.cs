using _Scripts.Entities.EntityStateMachine;
using _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates;
using _Scripts.ScriptableObjects.EntityData;

namespace _Scripts.Entities.EntitySpecific
{
    public class E1LongRangeAttackState : EntityRangedAttackState
    {
        private Entity1 _enemy;
    
        public E1LongRangeAttackState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName, Entity1 enemy) : base(entity, stateMachine, entityData, animBoolName)
        {
            this._enemy = enemy;
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
                StateMachine.ChangeState(_enemy.PlayerDetectedState);
            }
            else
            {
                StateMachine.ChangeState(_enemy.IdleState);
            }
        }
    }
}

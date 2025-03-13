using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;

namespace _Scripts.Entities.EntitySpecific
{
    public class E1PlayerDetectedState : EntityPlayerDetectedState
    {
        private Entity1 _enemy;
    
        public E1PlayerDetectedState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName, Entity1 enemy) : base(entity, stateMachine, entityData, animBoolName)
        {
            this._enemy = enemy;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (PerformLongRangeAction)
            {
                StateMachine.ChangeState(_enemy.LongRangeAttackState);
            }
            else if (!IsPlayerInLineOfSight)
            {
                StateMachine.ChangeState(_enemy.IdleState);
            }
        }
    }
}

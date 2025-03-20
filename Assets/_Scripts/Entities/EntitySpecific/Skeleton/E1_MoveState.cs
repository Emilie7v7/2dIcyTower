using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;
using EntityMoveState = _Scripts.Entities.EntityStates.EntitySubStates.EntityMovementStates.EntityMoveState;

namespace _Scripts.Entities.EntitySpecific.Skeleton
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
            
            _enemy.SkeletonAudio.PlayWalkingSound();
        }

        public override void Exit()
        {
            base.Exit();
            
            _enemy.SkeletonAudio.StopWalkingSound();
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

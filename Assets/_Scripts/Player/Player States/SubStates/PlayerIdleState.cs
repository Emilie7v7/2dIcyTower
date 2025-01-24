using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;

namespace _Scripts.Player.Player_States
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) : base(player, stateMachine, playerDataSo, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
            Movement?.SetZeroVelocity();
        }
        
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsExitingState)
            {
                if (XInput != 0f)
                {
                    StateMachine.ChangeState(Player.MoveState);
                }
            }
        }
    }
}

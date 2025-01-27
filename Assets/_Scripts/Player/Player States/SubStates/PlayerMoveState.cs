using _Scripts.PlayerComponent;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) : base(player, stateMachine, playerDataSo, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        Movement?.CheckIfShouldFlip(XInput);
        
        Movement?.SetVelocityX(PlayerData.PlayerMovementSpeed * XInput);

        if (!IsExitingState)
        {
            if (XInput == 0)
            {
                StateMachine.ChangeState(Player.IdleState);
            }
        }
    }
}

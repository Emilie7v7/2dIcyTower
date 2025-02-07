using _Scripts.Player.Player_States.SuperStates;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

namespace _Scripts.Player.Player_States.SubStates
{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) : base(player, stateMachine, playerDataSo, animBoolName)
        {
        }

        // public override void LogicUpdate()
        // {
        //     base.LogicUpdate();
        //
        //     Movement?.CheckIfShouldFlip(XInput);
        //
        //     Movement?.SetVelocityX(PlayerData.playerMovementSpeed * XInput);
        //
        //     if (!IsExitingState)
        //     {
        //         if (XInput == 0)
        //         {
        //             StateMachine.ChangeState(Player.IdleState);
        //         }
        //     }
        // }
    }
}

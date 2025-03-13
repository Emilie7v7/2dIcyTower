using _Scripts.Player.Player_States.SuperStates;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

namespace _Scripts.Player.Player_States
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) : base(player, stateMachine, playerDataSo, animBoolName)
        {
        }
        
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsGrounded)
            {
                StateMachine.ChangeState(Player.InAirState);
                return;
            }

            // Gradually reduce movement velocity instead of setting it to zero instantly
            if (Movement.CurrentVelocity.magnitude > 0.1f) // Allow a small threshold to avoid floating values
            {
                var newVelocityX = Mathf.Lerp(Movement.CurrentVelocity.x, 0f, PlayerData.decelerationRate * Time.deltaTime);
                Movement.SetVelocityX(newVelocityX);
            }
            else
            {
                Movement.SetVelocityX(0f);
            }
        }
    }
}

using _Scripts.CoreSystem;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

namespace _Scripts.Player.Player_States.SubStates
{
    public class PlayerInAirState : PlayerState.PlayerState
    {
        private bool _isThrowing;
        private bool _isGrounded;
        
        private float _startingYPosition;
        private bool _isFallingTriggered;
        
        private Movement Movement => _movement ? _movement : Core.GetCoreComponent<Movement>();
        private Movement _movement;

        private CollisionSenses CollisionSenses => _collisionSenses ? _collisionSenses : Core.GetCoreComponent<CollisionSenses>();
        private CollisionSenses _collisionSenses;

        public PlayerInAirState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) 
            : base(player, stateMachine, playerDataSo, animBoolName)
        {
        }

        public override void DoChecks()
        {
            base.DoChecks();

            if (CollisionSenses)
            {
                _isGrounded = CollisionSenses.Grounded;
            }
        }
        
        public override void Enter()
        {
            base.Enter();

            if (Player.WasThrowingInAir)
            {
                // **Prevent resetting movement when re-entering from throw**
                Player.WasThrowingInAir = false; // Reset flag
            }
            // else
            // {
            //     // **Apply normal gravity settings if NOT coming from a throw**
            //     Movement.R2BD.gravityScale = PlayerData.gravityScaleUp;
            // }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            _isThrowing = Player.InputHandler.ThrowInput;
            
            float verticalDistance = Movement.Rb2D.position.y - _startingYPosition; // Measure height gained

            if (Movement != null)
            {
                if (Movement.CurrentVelocity.y > 0.1f) // Moving Up
                {
                    // **Modify the gravity increase rate**
                    float heightFactor = Mathf.Pow(verticalDistance / PlayerData.maxUpwardDistance, PlayerData.gravityIncreaseFactor); // Slower gravity increase
                    float newGravity = Mathf.Lerp(PlayerData.gravityScaleUp, PlayerData.gravityScaleDown, heightFactor);

                    Movement.Rb2D.gravityScale = Mathf.Clamp(newGravity, PlayerData.gravityScaleUp, PlayerData.gravityScaleDown);
                    Movement.Rb2D.drag = PlayerData.dragUp;
                }
                else if (Movement.CurrentVelocity.y < -0.1f) // Falling
                {
                    Movement.Rb2D.gravityScale = PlayerData.gravityScaleDown;
                    Movement.Rb2D.drag = PlayerData.dragDown;
                }
            }

            if (_isGrounded && Movement?.CurrentVelocity.y < 0.01f)
            {
                StateMachine.ChangeState(Player.IdleState);
            }
            else if (_isThrowing)
            {
                StateMachine.ChangeState(Player.ThrowState);
            }
        }
        
        private void ResetGravity()
        {
            Movement.Rb2D.gravityScale = PlayerData.defaultGravityScale; // Reset to normal gravity
            Movement.Rb2D.drag = 0f; // Reset drag
        }
    }
}

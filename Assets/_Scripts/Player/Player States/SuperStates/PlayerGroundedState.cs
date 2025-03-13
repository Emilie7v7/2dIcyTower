using _Scripts.CoreSystem;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

namespace _Scripts.Player.Player_States.SuperStates
{
    public class PlayerGroundedState : PlayerState.PlayerState
    {
        private bool _jumpInput;
    
        //Checks
        protected bool IsGrounded;
        private bool _isThrowing;
    
        protected PlayerGroundedState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) : base(player, stateMachine, playerDataSo, animBoolName)
        {
        }

        protected Movement Movement => _movement ? _movement : Core.GetCoreComponent(ref _movement);
        private Movement _movement;

        private CollisionSenses CollisionSenses => _collisionSenses ? _collisionSenses : Core.GetCoreComponent(ref _collisionSenses);
        private CollisionSenses _collisionSenses;

        public override void DoChecks()
        {
            base.DoChecks();

            if (CollisionSenses)
            {
                IsGrounded = CollisionSenses.Grounded;
            }
        }

        public override void Enter()
        {
            base.Enter();
            ResetGravity(); // Reset gravity when touching the ground
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
    
            _isThrowing = Player.InputHandler.ThrowInput;

            if (_isThrowing)
            {
                StateMachine.ChangeState(Player.ThrowState);
            }
        }
        
        private void ResetGravity()
        {
            if (Movement is not null)
            {
                Movement.Rb2D.gravityScale = PlayerData.defaultGravityScale; // Reset gravity scale
                Movement.Rb2D.drag = 0f; // Reset drag
            }
        }
    }
}

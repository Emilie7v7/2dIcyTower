using _Scripts.CoreSystem;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

namespace _Scripts.Player.Player_States.SuperStates
{
    public class PlayerGroundedState : PlayerState.PlayerState
    {
        //Input
        //protected int XInput;
        //protected int YInput;
    
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
        
            //XInput = Player.InputHandler.NormInputX;
            //YInput = Player.InputHandler.NormInputY;
            _isThrowing = Player.InputHandler.ThrowInput;

            if (_isThrowing)
            {
                StateMachine.ChangeState(Player.ThrowState);
            }
        }
        
        private void ResetGravity()
        {
            if (Movement != null)
            {
                Movement.R2BD.gravityScale = PlayerData.defaultGravityScale; // Reset gravity scale
                Movement.R2BD.drag = 0f; // Reset drag
            }
        }
    }
}

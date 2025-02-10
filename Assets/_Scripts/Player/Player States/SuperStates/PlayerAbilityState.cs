using _Scripts.CoreSystem;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

namespace _Scripts.Player.Player_States.SuperStates
{
    public class PlayerAbilityState : PlayerState.PlayerState
    {
        private bool _isGrounded;
        private float _startingYPosition; // Store the initial height when entering the state

        protected Movement Movement => _movement ? _movement : Core.GetCoreComponent<Movement>();
        private Movement _movement;

        private CollisionSenses CollisionSenses => _collisionSenses ? _collisionSenses : Core.GetCoreComponent<CollisionSenses>();
        private CollisionSenses _collisionSenses;

        protected PlayerAbilityState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) 
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

            _startingYPosition = Movement.R2BD.position.y; // Store the height at the moment of throwing
            Player.WasThrowingInAir = !_isGrounded; // **Mark whether the player was in air when throwing**
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // **Ensure animation finishes before transitioning**
            if (!IsAnimationFinished) return;

            if (_isGrounded)
            {
                StateMachine.ChangeState(Player.IdleState);
            }
            else
            {
                StateMachine.ChangeState(Player.InAirState);
            }
        }
    }
}

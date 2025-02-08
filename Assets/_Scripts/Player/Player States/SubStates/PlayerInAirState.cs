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
            Debug.Log("We have entered the in-air state");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            _isThrowing = Player.InputHandler.ThrowInput;

            if (_isGrounded && Movement?.CurrentVelocity.y < 0.01f)
            {
                StateMachine.ChangeState(Player.IdleState);
            }
            else if (_isThrowing)
            {
                StateMachine.ChangeState(Player.ThrowState);
            }
        }
    }
}

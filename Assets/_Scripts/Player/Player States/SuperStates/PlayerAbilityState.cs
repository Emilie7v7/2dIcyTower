using _Scripts.CoreSystem;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Player.Player_States.SuperStates
{
    public class PlayerAbilityState : PlayerState.PlayerState
    {
        private bool _isGrounded;
        
        protected Movement Movement => _movement ? _movement : Core.GetCoreComponent<Movement>();
        private Movement _movement;

        private CollisionSenses CollisionSenses => _collisionSenses ? _collisionSenses : Core.GetCoreComponent<CollisionSenses>();
        private CollisionSenses _collisionSenses;
        
        protected PlayerAbilityState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) : base(player, stateMachine, playerDataSo, animBoolName)
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

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!IsAnimationFinished) return;

            // Use the latest _isGrounded value from DoChecks()
            if (_isGrounded)
            {
                Debug.Log("Ensuring transition to IdleState from ThrowState");
                StateMachine.ChangeState(Player.IdleState);
            }
            else
            {
                Debug.Log("Ensuring transition to InAirState from ThrowState");
                StateMachine.ChangeState(Player.InAirState);
            }
        }
    }
}

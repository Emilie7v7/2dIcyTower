using _Scripts.CoreSystem;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

namespace _Scripts.PlayerState
{
    public class PlayerState
    {
        protected readonly Core Core;

        protected readonly PlayerComponent.Player Player;
        protected readonly PlayerStateMachine StateMachine;
        protected readonly PlayerDataSo PlayerData;
    
        protected float StartTime;

        protected bool IsAnimationFinished;
        protected bool IsExitingState;

        private readonly string _animBoolName;

        protected PlayerState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerData, string animBoolName)
        {
            Player = player;
            StateMachine = stateMachine;
            PlayerData = playerData;
            this._animBoolName = animBoolName;
            Core = player.Core;
        }

        //Gets called when we enter a specific state
        public virtual void Enter()
        {
            DoChecks();
            Player.MyAnimator.SetBool(_animBoolName, true);
            StartTime = Time.time;
            IsAnimationFinished = false;
            IsExitingState = false;
        }

        //Gets called when we exit a specific state
        public virtual void Exit()
        {
            Player.MyAnimator.SetBool(_animBoolName, false);
            IsExitingState = true;
        }

        //Gets called every frame (Update)
        public virtual void LogicUpdate() {}
    
        //Gets called every frame (FixedUpdate)
        public virtual void PhysicsUpdate()
        {
            DoChecks();
        }
        
        //Is a function that gets called from a PhysicsUpdate and from an Enter function
        public virtual void DoChecks() { }

        public virtual void AnimationTrigger() { }

        public virtual void AnimationFinishTrigger() => IsAnimationFinished = true;
    }
}

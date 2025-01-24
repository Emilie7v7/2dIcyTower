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

        private protected string AnimBoolName;
    
        protected PlayerState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName )
        {
            Player = player;
            StateMachine = stateMachine;
            PlayerData = playerDataSo;
            this.AnimBoolName = animBoolName;
            Core = player.Core;
        }
        
        //Being called whenever we enter a state
        public virtual void Enter()
        {
            DoChecks();
            StartTime = Time.time;
            IsAnimationFinished = false;
            IsExitingState = false;
        }
        //Being called whenever we exit a state
        public virtual void Exit()
        {
            IsExitingState = true;
        }

        //Function that is called in Update()
        public virtual void LogicUpdate() { }
        
        //Function that is called in FixedUpdate()
        public virtual void PhysicsUpdate() { DoChecks(); }
        
        //Function that is called in Enter and PhysicsUpdate() to check collisions
        public virtual void DoChecks() { }

        public virtual void AnimationTrigger() { }
        public virtual void AnimationFinishTrigger() => IsAnimationFinished = true;
    }
}

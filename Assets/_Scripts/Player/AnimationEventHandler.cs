using System;
using _Scripts.Player.Player_States.SuperStates;
using UnityEngine;

namespace _Scripts.Player
{
    public class AnimationEventHandler : MonoBehaviour
    {
        public PlayerAbilityState PlayerAbilityState;

        private void AttackActionTrigger()
        {
            PlayerAbilityState.AnimationTrigger();
        }

        private void AnimationFinishTrigger()
        {
            PlayerAbilityState.AnimationFinishTrigger();
        }
    }
}

using _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates;
using UnityEngine;

namespace _Scripts.Intermediaries
{
    public class AnimationToStateMachine : MonoBehaviour
    {
        public EntityAttackState EntityAttackState;

        private void AnimationAttackTrigger()
        {
            EntityAttackState.AnimationAttackTrigger();
        }

        private void AnimationFinishTrigger()
        {
            EntityAttackState.AnimationFinishedTrigger();
        }
    }
}

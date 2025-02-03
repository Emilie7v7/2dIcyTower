using System.Collections;
using System.Collections.Generic;
using _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates;
using UnityEngine;

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

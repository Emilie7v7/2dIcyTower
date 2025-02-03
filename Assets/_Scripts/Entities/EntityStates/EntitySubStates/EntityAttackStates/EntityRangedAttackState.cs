using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates
{
    public class EntityRangedAttackState : EntityAttackState
    {
    
        public EntityRangedAttackState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }

        public override void AnimationAttackTrigger()
        {
            base.AnimationAttackTrigger();

            Debug.Log("Entity shot at player!!");
        }
    }
}

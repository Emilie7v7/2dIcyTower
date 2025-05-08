using _Scripts.Entities.EntityStateMachine;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.Projectiles;
using _Scripts.ScriptableObjects.EntityData;
using _Scripts.ScriptableObjects.ProjectileData;
using UnityEngine;

namespace _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates
{
    public class EntityRangedAttackState : EntityAttackState
    {
        protected EntityRangedAttackState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }
        
               
    }
}

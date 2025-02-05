using _Scripts.CoreSystem;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntityStates.EntitySubStates.EntityAttackStates
{
    public class EntityRangedAttackState : EntityAttackState
    {
        protected GameObject Projectile;

        protected EntityRangedAttackState(Entity entity, EntityStateMachine.EntityStateMachine stateMachine, EntityDataSo entityData, string animBoolName) : base(entity, stateMachine, entityData, animBoolName)
        {
        }
        
        public override void AnimationAttackTrigger()
        {
            base.AnimationAttackTrigger();
            
            // Find the player
            var player = GameObject.FindGameObjectWithTag("Player"); 

            // Instantiate the projectile
            var projectileInstance = Object.Instantiate(EntityData.projectilePrefab, Entity.transform.position, Quaternion.identity);
            
            // Get the Projectile component
            var projectileScript = projectileInstance.GetComponent<Projectile.Projectile>();

            // Calculate direction towards the player
            Vector2 direction = (player.transform.position - Entity.transform.position).normalized;

            // Add an upward bias to create an arc
            direction += projectileScript.ProjectileData.projectileArc;

            //Debug.Log($"Projectile will move towards direction: {direction}");
            
            // Launch the projectile with force (allowing for an arc due to gravity)
            projectileScript.Movement.LaunchProjectile(direction, projectileScript.ProjectileData.projectileSpeed);
        }       
    }
}

using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.Player.Player_States.SuperStates;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using _Scripts.Projectiles;
using _Scripts.ScriptableObjects.ProjectileData;
using UnityEngine;

namespace _Scripts.Player.Player_States.SubStates
{
    public class PlayerThrowState : PlayerAbilityState
    {
        // Correcting for the -45 degree offset
        private static readonly Quaternion CorrectionRotation = Quaternion.Euler(0, 0, 45f);    // Reversing the -45f applied in Update()

        public PlayerThrowState(PlayerComponent.Player player, PlayerStateMachine stateMachine, PlayerDataSo playerDataSo, string animBoolName) : base(player, stateMachine, playerDataSo, animBoolName)
        {
        }
        
        public override void AnimationTrigger()
        {
            base.AnimationTrigger();
            
            var projectilePrefab = PlayerProjectilePool.Instance.GetObject(Player.transform.position);
            if (projectilePrefab is null) return;
            
            projectilePrefab.SetProjectileOwner(true);
            projectilePrefab.transform.position = Player.transform.position;
            
            Vector2 launchDirection = CorrectionRotation * Player.ThrowDirectionIndicator.transform.right; 

            projectilePrefab.Movement.LaunchProjectile(launchDirection, projectilePrefab.ProjectileData.projectileSpeed);
        }
    }
}

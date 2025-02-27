using _Scripts.Player.Player_States.SuperStates;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
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
            
            var projectilePrefab = Object.Instantiate(PlayerData.projectilePrefab, Player.transform.position, Quaternion.identity);
            var projectileScript = projectilePrefab.GetComponent<Projectile.Projectile>();
            
            Vector2 launchDirection = CorrectionRotation * Player.ThrowDirectionIndicator.transform.right; 

            projectileScript.Movement.LaunchProjectile(launchDirection, projectileScript.ProjectileData.projectileSpeed);
        }
    }
}

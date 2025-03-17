using _Scripts.ObjectPool.ObjectsToPool;
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

        public override void Enter()
        {
            base.Enter();

        }

        public override void AnimationTrigger()
        {
            base.AnimationTrigger();
            

            var projectilePrefab = PlayerProjectilePool.Instance.GetObject(Player.transform.position);
            if (projectilePrefab is null)
            {
                Debug.LogError("Projectile is NULL! Pool might be empty.");
                return;
            }
            
            projectilePrefab.SetProjectileOwner(true);
            projectilePrefab.transform.position = Player.transform.position;
            
            Vector2 launchDirection = CorrectionRotation * Player.ThrowDirectionIndicator.transform.right; 

            projectilePrefab.Movement.LaunchProjectile(launchDirection, projectilePrefab.ProjectileData.projectileSpeed);
            
            ApplyBackBlastEffect(launchDirection);
        }
        private void ApplyBackBlastEffect(Vector2 launchDirection)
        {
            if (Player == null) return;

            const float backBlastForce = 5f; // Adjust this value as needed

            var backBlastDirection = new Vector2(-launchDirection.x, 0); // Opposite direction, no Y movement
            Player.Movement.Rb2D.AddForce(backBlastDirection * backBlastForce, ForceMode2D.Impulse);
        }
    }
}

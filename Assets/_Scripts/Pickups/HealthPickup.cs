using _Scripts.CoreSystem;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.ObjectPool.ObjectsToPool;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class HealthPickup : PowerUp
    {
        [SerializeField] private ParticleSystem healthEffect;
        
        private void Update()
        {
            if(Player is null) return;
            
            MoveTowardsPlayer();
        }
        private void MoveTowardsPlayer()
        {
            var direction = (Player.position - transform.position).normalized;
            var distance = Vector3.Distance(Player.position, transform.position);
            
            // Normalize distance (0 = near player, 1 = far from player)
            var distanceFromPlayer = Mathf.Clamp01(1 - (distance / 300));

            var smoothPullSpeed = maxPullSpeed * (0.5f * (1 - Mathf.Cos(distanceFromPlayer * Mathf.PI)));

            // Apply movement
            transform.position += smoothPullSpeed * Time.deltaTime * direction;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                healthEffect.Play();
                var player = other.GetComponentInChildren<Stats>();
                if (player.Health.CurrentValue < player.Health.MaxValue)
                {
                    player.Health.IncreaseAmount(1);
                }
                PowerUpPool.Instance.ReturnObject(this);
            }
        }
        private void OnEnable()
        {
            if (Player is null)
            {
                Player = GameManager.Instance?.Player;
            }
        }
    }
}

using _Scripts.CoreSystem;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class HealthPickup : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInChildren<Stats>();
                if (player.Health.CurrentValue < player.Health.MaxValue)
                {
                    player.Health.IncreaseAmount(1);
                }
                Destroy(gameObject);
            }
        }
    }
}

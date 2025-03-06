using _Scripts.Managers.GameManager;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Pickups
{
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private ObjectSpawnSettingsSo settings;
        [SerializeField] private Transform player;
        private bool _isBeingPulled = false;

        private void Update()
        {
            if (_isBeingPulled)
            {
                var direction = (player.position - transform.position).normalized;
                var distance = Vector3.Distance(player.position, transform.position);
                
                // Normalize distance (0 = near player, 1 = far from player)
                var distanceFromPlayer = Mathf.Clamp01(1 - (distance / 300));

                // Ease-in, ease-out using cosine function
                const float maxPullSpeed = 80f; // Maximum pull speed
                var smoothPullSpeed = maxPullSpeed * (0.5f * (1 - Mathf.Cos(distanceFromPlayer * Mathf.PI)));

                // Apply movement
                transform.position += smoothPullSpeed * Time.deltaTime * direction;
            }
        }

        public void StartPulling()
        {
            _isBeingPulled = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.AddCoins(settings.coinValue);

                if (CoinPool.Instance != null) 
                {
                    CoinPool.Instance.ReturnObject(this);
                }
            }
        }
    }
}

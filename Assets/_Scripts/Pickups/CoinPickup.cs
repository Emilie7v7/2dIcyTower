using _Scripts.Managers.GameManager;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private int coinValue = 1;
        private bool _isBeingPulled = false;
        private Transform _player;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (_isBeingPulled)
            {
                var direction = (_player.position - transform.position).normalized;
                var distance = Vector3.Distance(_player.position, transform.position);
                
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
                GameManager.Instance.AddCoins(coinValue);
                Destroy(gameObject);
            }
        }
    }
}

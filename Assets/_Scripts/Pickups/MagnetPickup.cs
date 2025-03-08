using System.Collections;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.ObjectPool.ObjectsToPool;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Pickups
{
    public class MagnetPickup : PowerUp
    {
        [SerializeField] private float horizontalMagnetRange = 300f; // Wider horizontal range
        [SerializeField] private float verticalMagnetRange = 50f; // Shorter vertical range
        private float _magnetDuration; // Will be fetched from PlayerData
        private bool _isMagnetActive = false;
        
        private void Start()
        {
            _magnetDuration = GameManager.Instance.PlayerData.magnetDuration;
        }

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

            // Ease-in, ease-out using cosine function
            var smoothPullSpeed = maxPullSpeed * (0.5f * (1 - Mathf.Cos(distanceFromPlayer * Mathf.PI)));

            // Apply movement
            transform.position += smoothPullSpeed * Time.deltaTime * direction;
        }
        
        protected override void Activate()
        {
            if (_isMagnetActive) return;

            _isMagnetActive = true;
            transform.SetParent(Player.transform); // Attach to the player
            transform.localPosition = Vector3.zero; // Center it on the player

            GetComponent<Collider2D>().enabled = false; // Disable pickup collider
            GetComponent<SpriteRenderer>().enabled = false; // Hide visual magnet

            StartCoroutine(MagnetEffect());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Activate();
            }
        }

        private IEnumerator MagnetEffect()
        {
            var elapsedTime = 0f;

            while (elapsedTime < _magnetDuration)
            {
                PullCoinsTowardsPlayer();
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _isMagnetActive = false; // Deactivate after duration ends
            PowerUpPool.Instance.ReturnObject(this);
        }

        private void PullCoinsTowardsPlayer()
        {
            var hitColliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(horizontalMagnetRange, verticalMagnetRange), 0f);

            foreach (var colliders in hitColliders)
            {
                if (colliders.CompareTag("Coin"))
                {
                    var coinScript = colliders.GetComponent<CoinPickup>();
                    coinScript?.StartPulling();
                }
            }
        }

        private void OnEnable()
        {
            if (Player == null && GameManager.Instance != null)
            {
                Player = GameManager.Instance.Player;
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(horizontalMagnetRange, verticalMagnetRange, 0f));
        }
    }
}
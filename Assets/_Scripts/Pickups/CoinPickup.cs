using System;
using System.Collections;
using _Scripts.Audio;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private ObjectSpawnSettingsSo settings;
        [SerializeField] private float getPulledDistance = 5f;
        [SerializeField] private float maxPullSpeed = 30f;
        [SerializeField] private bool isPhysicalCoin;
        
        private Collider2D _collider2D;
        private Rigidbody2D _rigidbody2D;
        private bool _isBeingPulled;
        private Transform _player;


        private void Start()
        {
            _collider2D = GetComponent<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (_player is null) return; // Avoid null reference errors

            var distance = Vector3.Distance(_player.position, transform.position);

            if (isPhysicalCoin)
            {
                StartCoroutine(BounceCoinsCoroutine());
            }
            if (!isPhysicalCoin)
            {
                if (distance < getPulledDistance)
                    _isBeingPulled = true;

                if (_isBeingPulled)
                    MoveTowardsPlayer(distance);
            }
        }

        private void MoveTowardsPlayer(float distance)
        {
            var direction = (_player.position - transform.position).normalized;

            // Normalize distance (0 = near player, 1 = far from player)
            var distanceFactor = Mathf.Clamp01(1 - (distance / 300));

            // Ease-in, ease-out using cosine function
            var smoothPullSpeed = maxPullSpeed * (0.5f * (1 - Mathf.Cos(distanceFactor * Mathf.PI)));

            // Apply movement
            transform.position += smoothPullSpeed * Time.deltaTime * direction;
        }

        private IEnumerator BounceCoinsCoroutine()
        {
            yield return new WaitForSeconds(2f);
            _rigidbody2D.gravityScale = 1f;
            _collider2D.isTrigger = true;
            _rigidbody2D.isKinematic = true;
            var distance = Vector3.Distance(_player.position, transform.position);
            MoveTowardsPlayer(distance);
        }
        
        public void StartPulling()
        {
            _isBeingPulled = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            other.GetComponent<PlayerAudio>()?.PlayCoinPickupSound();
            
            GameManager.Instance.AddCoins(settings.coinValue);
            CoinPool.Instance?.ReturnObject(this);
        }

        private void OnEnable()
        {
            if (_player == null)
            {
                _player = GameManager.Instance?.Player;
            }
            _isBeingPulled = false; // Reset pull state when re-enabled
        }
    }
}
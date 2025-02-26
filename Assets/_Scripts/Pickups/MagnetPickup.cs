using System;
using System.Collections;
using _Scripts.Managers.GameManager;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class MagnetPickup : MonoBehaviour
    {
        [SerializeField] private float magnetRange = 5f; // Static range, doesn't change
        private float _magnetDuration; // Will be fetched from PlayerData
        private bool _isMagnetActive = false;
        private Transform _player;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _magnetDuration = GameManager.Instance.PlayerData.magnetDuration;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                ActivateMagnet(other.transform);
            }
        }

        private void ActivateMagnet(Transform playerTransform)
        {
            if (_isMagnetActive) return;

            _isMagnetActive = true;
            transform.SetParent(playerTransform); // Attach to the player
            transform.localPosition = Vector3.zero; // Center it on the player

            GetComponent<Collider2D>().enabled = false; // Disable pickup collider
            GetComponent<SpriteRenderer>().enabled = false; // Hide visual magnet

            StartCoroutine(MagnetEffect());
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
            Destroy(gameObject); // Destroy the magnet pickup
        }

        private void PullCoinsTowardsPlayer()
        {
            var hitColliders = Physics2D.OverlapCircleAll(_player.position, magnetRange);

            foreach (var colliders in hitColliders)
            {
                if (colliders.CompareTag("Coin"))
                {
                    var coinTransform = colliders.transform;
                    var direction = (_player.position - coinTransform.position).normalized;
            
                    const float pullSpeed = 10f; // Adjust speed as needed
                    coinTransform.position += pullSpeed * Time.deltaTime * direction;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, magnetRange);
        }
    }
}
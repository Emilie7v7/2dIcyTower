using System;
using System.Collections;
using _Scripts.Managers.GameManager;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class MagnetPickup : MonoBehaviour
    {
        [SerializeField] private float horizontalMagnetRange = 300f; // Wider horizontal range
        [SerializeField] private float verticalMagnetRange = 50f; // Shorter vertical range
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
    var hitColliders = Physics2D.OverlapBoxAll(_player.position, new Vector2(horizontalMagnetRange, verticalMagnetRange), 0f);

    foreach (var colliders in hitColliders)
{
    if (colliders.CompareTag("Coin"))
{
    var coinTransform = colliders.transform;
    var direction = (_player.position - coinTransform.position).normalized;
    float distance = Vector3.Distance(_player.position, coinTransform.position);

    // Normalized distance (0 = near player, 1 = far from player)
    float t = Mathf.Clamp01(1 - (distance / horizontalMagnetRange));

    // Ease-in, ease-out using cosine
    const float maxPullSpeed = 80f; // Maximum pull speed
    float smoothPullSpeed = maxPullSpeed * (0.5f * (1 - Mathf.Cos(t * Mathf.PI)));

    // Apply pull movement
    coinTransform.position += smoothPullSpeed * Time.deltaTime * direction;
}
}
}

private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(transform.position, new Vector3(horizontalMagnetRange, verticalMagnetRange, 0f));
}
    }
}
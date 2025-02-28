using System;
using System.Collections;
using _Scripts.CoreSystem;
using _Scripts.InputHandler;
using _Scripts.Managers.GameManager;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class RocketPickup : MonoBehaviour
    {
        private float _rocketDuration;
        private const float RocketSpeed = 100f;
        private bool _isRocketActive = false;
        
        private Rigidbody2D _playerRb;
        private PlayerInputHandler _playerInput;
        private Stats _playerStats;


        private void Start()
        {
            _rocketDuration = GameManager.Instance.PlayerData.rocketBoostDuration;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerRb = other.GetComponent<Rigidbody2D>();
                _playerInput = other.GetComponent<PlayerInputHandler>();
                _playerStats = other.GetComponentInChildren<Stats>();
                
                if (_playerRb != null)
                {
                    ActivateRocketBoost(other.transform);
                    _playerStats.ActivateImmortality(_rocketDuration);
                }
            }
        }

        private void ActivateRocketBoost(Transform playerTransform)
        {
            if(_isRocketActive) return;
            
            _isRocketActive = true;
            transform.SetParent(playerTransform);
            transform.localPosition = Vector3.zero;
            
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            StartCoroutine(RocketEffect());
        }

        private IEnumerator RocketEffect()
        {
            var elapsedTime = 0f;

            while (elapsedTime < _rocketDuration)
            {
                _playerInput.CanThrow = false;
                _playerRb.velocity = new Vector2(_playerRb.velocity.x, RocketSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            _playerInput.CanThrow = true;
            _playerRb.velocity = new Vector2(_playerRb.velocity.x, 0);
            _isRocketActive = false;
            Destroy(gameObject);
        }
    }
}

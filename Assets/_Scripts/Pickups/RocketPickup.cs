using System;
using System.Collections;
using _Scripts.CoreSystem;
using _Scripts.InputHandler;
using _Scripts.Managers.GameManager;
using _Scripts.ObjectPool.ObjectsToPool;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class RocketPickup : PowerUp
    {
        private float _rocketDuration;
        private const float RocketSpeed = 100f;
        private bool _isRocketActive = false;
        
        private Rigidbody2D _playerRb;
        private PlayerInputHandler _playerInput;
        private Stats _playerStats;
        private Transform _player;


        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
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
                    Activate();
                    _playerStats.ActivateImmortality(_rocketDuration);
                }
            }
        }

        protected override void Activate()
        {
            base.Activate();
            
            if(_isRocketActive) return;
            
            _isRocketActive = true;
            transform.SetParent(_playerRb.transform);
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
            _playerRb.velocity = new Vector2(_playerRb.velocity.x, _playerRb.velocity.y);
            _isRocketActive = false;
            PowerUpPool.Instance.ReturnObject(this);
        }
    }
}

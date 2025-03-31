using System.Collections;
using _Scripts.CoreSystem;
using _Scripts.InputHandler;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.Managers.Score_Logic;
using _Scripts.ObjectPool.ObjectsToPool;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class RocketPickup : PowerUp
    {
        private float _rocketDuration;
        private const float RocketSpeed = 75f;
        private bool _isRocketActive = false;
        
        private Rigidbody2D _playerRb;
        private PlayerInputHandler _playerInput;
        private Stats _playerStats;
        
        private void Start()
        {
            _rocketDuration = GameManager.Instance.PlayerData.rocketBoostDuration;
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
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerRb = other.GetComponent<Rigidbody2D>();
                _playerInput = other.GetComponent<PlayerInputHandler>();
                _playerStats = other.GetComponentInChildren<Stats>();
                
                if (_playerRb is not null)
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
            ScoreManager.Instance.FreezeTimer(_rocketDuration);
            transform.SetParent(_playerRb.transform);
            transform.localPosition = Vector3.zero;
            
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            StartCoroutine(RocketEffect());
        }

        private IEnumerator RocketEffect()
        {
            var elapsedTime = 0f;
            const float frequency = 2f; // Controls oscillation speed
            const float moveToCenterDuration = 1f; // Duration to move to the center
            const float levelMiddleX = (-14f + 14f) / 2;

            // Smoothly move the player to the middle of the level
            var centerTime = 0f;
            var startPosition = _playerRb.position;
            var targetPosition = new Vector2(levelMiddleX, startPosition.y);

            while (centerTime < moveToCenterDuration)
            {
                // Smoothly move towards center
                var t = centerTime / moveToCenterDuration;
                _playerRb.position = new Vector2(Mathf.Lerp(startPosition.x, targetPosition.x, t), _playerRb.position.y);
                _playerRb.velocity = new Vector2(_playerRb.velocity.x, RocketSpeed);
                centerTime += Time.deltaTime;
                Physics.IgnoreLayerCollision(3, 14, true);
                yield return null;
            }
            var finalVelocity = _playerRb.velocity.y;

            // Start oscillating but based on level boundaries
            while (elapsedTime < _rocketDuration)
            {
                _playerInput.CanThrow = false;

                // Map Sin to the entire level width
                var normalizedSin = Mathf.Sin(elapsedTime * frequency);
                var horizontalPosition = Mathf.Lerp(-10, 10, (normalizedSin + 1) / 2);

                // Apply horizontal movement while still boosting upwards
                _playerRb.position = new Vector2(horizontalPosition, _playerRb.position.y + RocketSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                Physics.IgnoreLayerCollision(3, 14, true);
                yield return null;
            }

            // Reset movement after effect
            _playerInput.CanThrow = true;
            _playerRb.velocity = new Vector2(_playerRb.velocity.x, finalVelocity);
            _isRocketActive = false;
            Physics.IgnoreLayerCollision(3, 14, false);
            PowerUpPool.Instance.ReturnObject(this);
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

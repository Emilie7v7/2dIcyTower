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
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private ParticleSystem rocketEffect;

        private float _rocketDuration;
        private const float RocketSpeed = 20f;
        private bool _isRocketActive = false;

        private Rigidbody2D _playerRb;
        private PlayerInputHandler _playerInput;
        private Stats _playerStats;

        private const int PlayerLayer = 3;
        private int[] _hazardLayers = {14, 21, 22, 23, 24};

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
                    _playerStats.ActivateImmortality(_rocketDuration + 1f);
                }
            }
        }

        protected override void Activate()
        {
            base.Activate();
            
            if(_isRocketActive) return;
            
            _isRocketActive = true;
            rocketEffect.Play();
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
            const float moveCenterSpeed = 10f; // X smoothing speed
            const float centerX = 0f;

            foreach (var layers in _hazardLayers)
            {
                Physics2D.IgnoreLayerCollision(PlayerLayer, layers, true);
            }
            
            bool shouldContinueBoost;
            do
            {
                _playerInput.CanThrow = false;
                
                // Smooth horizontal drift to center
                var directionX = Mathf.Sign(centerX - _playerRb.position.x);
                var deltaX = Mathf.Abs(centerX - _playerRb.position.x);

                var xSpeed = (deltaX > 0.05f) ? moveCenterSpeed * directionX : 0f;

                // Apply velocity directly
                _playerRb.velocity = new Vector2(xSpeed, RocketSpeed);

                elapsedTime += Time.deltaTime;
                
                // Check if we're inside a solid platform (layer 14)
                var boxSize = new Vector2(0.9f, 1.8f);
                var solidPlatformLayer = 1 << 14; // Layer 14
                shouldContinueBoost = elapsedTime >= _rocketDuration && 
                                     Physics2D.OverlapBox(_playerRb.position, boxSize, 0f, solidPlatformLayer);
                
                yield return null;
            } while (elapsedTime < _rocketDuration || shouldContinueBoost);

            // Don't override Y — preserve current momentum
            _isRocketActive = false;
            
            yield return StartCoroutine(WaitUntilNotInsidePlatform());
            foreach (var layers in _hazardLayers)
            {
                Physics2D.IgnoreLayerCollision(PlayerLayer, layers, false);
            }
            rocketEffect.Stop();
            _playerInput.CanThrow = true;
            PowerUpPool.Instance.ReturnObject(this);
        }
        
        private IEnumerator WaitUntilNotInsidePlatform()
        {
            const float checkInterval = 0.05f;
            const float maxWaitTime = 2f;
            var waitedTime = 0f;

            
            var boxSize = new Vector2(0.9f, 1.8f);
            
            while (waitedTime < maxWaitTime)
            {
                // Check if still overlapping any platform
                var hit = Physics2D.OverlapBox(_playerRb.position, boxSize, 0f, layerMask);

                if (!hit) break; // No longer inside a platform

                waitedTime += checkInterval;
                yield return new WaitForSeconds(checkInterval);
            }
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
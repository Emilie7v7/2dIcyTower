using System;
using _Scripts.Managers.Game_Manager_Logic;
using UnityEngine;

namespace _Scripts.Managers.Score_Logic
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public event Action<int> OnScoreUpdated;
        public event Action<float> OnMultiplierUpdated;
        public event Action<float, float> OnKillstreakTimeUpdated;
        
        private int _score;
        private int _maxMultiplierLevel;
        
        private float _multiplier = 1.0f;
        private float _multiplierTimeLeft;
        private float _initialPosition;
        private const float BaseMultiplierDuration = 6.5f;

        [SerializeField] private Transform playerTransform;
        private float _lastRecordedHeight;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            _maxMultiplierLevel = GameManager.Instance.PlayerData.killstreakMultiplier;
            _lastRecordedHeight = playerTransform.position.y; // Initialize height tracking
            InvokeRepeating(nameof(UpdateScore), 0.1f, 0.1f);
        }

        private void Update()
        {
            if (_multiplier > 1f)
            {
                _multiplierTimeLeft -= Time.deltaTime;
                OnKillstreakTimeUpdated?.Invoke(_multiplierTimeLeft, BaseMultiplierDuration);
                if (_multiplierTimeLeft <= 0)
                {
                    ResetMultiplier();
                }
            }
        }
        private void UpdateScore()
        {
            var currentHeight = playerTransform.position.y;

            // Calculate height difference
            var heightDifference = currentHeight - _lastRecordedHeight;
    
            // If the player moved up, accumulate score
            if (heightDifference > 0)
            {
                var scoreIncrease = Mathf.FloorToInt(heightDifference * 10 * _multiplier);
                _score += scoreIncrease;
                OnScoreUpdated?.Invoke(_score);
        
                // Update last recorded height
                _lastRecordedHeight = currentHeight;
            }
        }

        public void RegisterKill()
        {
            if (_multiplier < _maxMultiplierLevel)
            {
                _multiplier += 0.25f; // Increase multiplier only if it's below max
            }

            _multiplierTimeLeft = BaseMultiplierDuration; // Always reset timer

            OnMultiplierUpdated?.Invoke(_multiplier);
            OnKillstreakTimeUpdated?.Invoke(_multiplierTimeLeft, BaseMultiplierDuration);
        }

        private void ResetMultiplier()
        {
            _multiplier = 1.0f;
            OnMultiplierUpdated?.Invoke(_multiplier);
            OnKillstreakTimeUpdated?.Invoke(0, BaseMultiplierDuration);

        }
        
        public int GetScore()
        {
            return _score;
        }

        public float GetMultiplier()
        {
            return _multiplier;
        }
    }
}

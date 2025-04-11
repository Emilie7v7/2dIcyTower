using System;
using System.Collections;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.Managers.Save_System_Logic;
using UnityEngine;

namespace _Scripts.Managers.Score_Logic
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public event Action<int> OnScoreUpdated;
        public event Action<float> OnMultiplierUpdated;
        public event Action<float, float> OnKillstreakTimeUpdated;
        public event Action<int, int> OnDeathScoreUpdated;
        
        private int _score;
        private int _maxMultiplierLevel;

        private float _multiplier = 0f;
        private float _startMultiplier;
        private float _multiplierTimeLeft;
        private float _initialPosition;
        private const float BaseMultiplierDuration = 6.5f;
        
        private bool isTimerFrozen = false;
        
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
            if (GameManager.Instance.PlayerData != null)
            {
                _multiplier += GameManager.Instance.PlayerData.multiplierUpgrade;
            }
            _startMultiplier = _multiplier;
            RunStarted();
            InvokeRepeating(nameof(UpdateScore), 0.1f, 0.1f);
        }

        private void Update()
        {
            if (!isTimerFrozen && _multiplier > _startMultiplier)
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

        public void FreezeTimer(float freezeDuration)
        {
            if (!isTimerFrozen)
            {
                StartCoroutine(FreezeTimerCoroutine(freezeDuration));
            }
        }

        private IEnumerator FreezeTimerCoroutine(float freezeDuration)
        {
            isTimerFrozen = true;
            yield return new WaitForSeconds(freezeDuration);
            isTimerFrozen = false;
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
            _multiplier = _startMultiplier;
            OnMultiplierUpdated?.Invoke(_multiplier);
            OnKillstreakTimeUpdated?.Invoke(0, BaseMultiplierDuration);
        }
        
        private void RunStarted()
        {
            _maxMultiplierLevel = GameManager.Instance.PlayerData.killstreakMultiplier + GameManager.Instance.PlayerData.multiplierUpgrade;
            _lastRecordedHeight = playerTransform.position.y + 1;
        }
        
        public int GetScore()
        {
            return _score;
        }
        public float GetMultiplier()
        {
            return _multiplier;
        }

        public void OnDeathScoreUpdatedEvent()
        {
            OnDeathScoreUpdated?.Invoke(GameManager.Instance.PlayerData.highScore, GetScore());
        }
    }
}

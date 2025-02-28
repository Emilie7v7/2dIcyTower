using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Managers.ScoreManager
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
        private float _baseMultiplierDuration = 5.0f;
        
        private Transform _playerTransform;

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
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            _maxMultiplierLevel = GameManager.GameManager.Instance.PlayerData.killstreakMultiplier;
            
            InvokeRepeating(nameof(UpdateScore), 0.1f, 0.1f);
            Debug.Log(_maxMultiplierLevel);
        }

        private void Update()
        {
            if (_multiplier > 1f)
            {
                _multiplierTimeLeft -= Time.deltaTime;
                OnKillstreakTimeUpdated?.Invoke(_multiplierTimeLeft, _baseMultiplierDuration);
                if (_multiplierTimeLeft <= 0)
                {
                    ResetMultiplier();
                }
            }
        }

        private void UpdateScore()
        {
            var newScore = Mathf.FloorToInt(_playerTransform.position.y * 10 * _multiplier);
            if (newScore > _score)
            {
                _score = newScore;
                OnScoreUpdated?.Invoke(_score);
            }
        }

        public void RegisterKill()
        {
            if (_multiplier < _maxMultiplierLevel)
            {
                _multiplier += 0.25f; // Increase multiplier only if it's below max
            }

            _multiplierTimeLeft = _baseMultiplierDuration; // Always reset timer

            OnMultiplierUpdated?.Invoke(_multiplier);
            OnKillstreakTimeUpdated?.Invoke(_multiplierTimeLeft, _baseMultiplierDuration);
        }

        private void ResetMultiplier()
        {
            _multiplier = 1.0f;
            OnMultiplierUpdated?.Invoke(_multiplier);
            OnKillstreakTimeUpdated?.Invoke(0, _baseMultiplierDuration);
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

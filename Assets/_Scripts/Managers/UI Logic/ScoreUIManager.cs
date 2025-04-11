using System;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.Managers.Score_Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers.UI_Logic
{
    public class ScoreUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private TMP_Text uponDeathScoreText;
        [SerializeField] private TMP_Text multiplierText;
        [SerializeField] private Slider killStreakTimerSlider;
        
        private void OnEnable()
        {
            ScoreManager.Instance.OnScoreUpdated += UpdateScore;
            ScoreManager.Instance.OnMultiplierUpdated += UpdateMultiplier;
            ScoreManager.Instance.OnKillstreakTimeUpdated += UpdateKillStreakTimer;
            ScoreManager.Instance.OnDeathScoreUpdated += UpdateScoreUIUponDeath;
        }

        private void OnDisable()
        {
            ScoreManager.Instance.OnScoreUpdated -= UpdateScore;
            ScoreManager.Instance.OnMultiplierUpdated -= UpdateMultiplier;
            ScoreManager.Instance.OnKillstreakTimeUpdated -= UpdateKillStreakTimer;
            ScoreManager.Instance.OnDeathScoreUpdated -= UpdateScoreUIUponDeath;
        }

        private void Start()
        {
            multiplierText.text = GameManager.Instance.PlayerData.multiplierUpgrade + "x";
        }

        private void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }

        private void UpdateMultiplier(float multiplier)
        {
            multiplierText.text = multiplier + "x";
        }

        private void UpdateKillStreakTimer(float timeLeft, float timeMax)
        {
            killStreakTimerSlider.value = timeLeft / timeMax;
        }
        
        private void UpdateScoreUIUponDeath(int currentHighScore, int uponDeathScore)
        {
            highScoreText.text = $"{currentHighScore}"; // Update UI
            uponDeathScoreText.text = $"{uponDeathScore}";
        }
    }
}

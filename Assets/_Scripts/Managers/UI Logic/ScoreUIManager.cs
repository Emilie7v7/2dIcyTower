using _Scripts.Managers.Score_Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers.UI_Logic
{
    public class ScoreUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text multiplierText;
        [SerializeField] private Slider killStreakTimerSlider;
        
        private void OnEnable()
        {
            ScoreManager.Instance.OnScoreUpdated += UpdateScore;
            ScoreManager.Instance.OnMultiplierUpdated += UpdateMultiplier;
            ScoreManager.Instance.OnKillstreakTimeUpdated += UpdateKillStreakTimer;
        }

        private void OnDisable()
        {
            ScoreManager.Instance.OnScoreUpdated -= UpdateScore;
            ScoreManager.Instance.OnMultiplierUpdated -= UpdateMultiplier;
            ScoreManager.Instance.OnKillstreakTimeUpdated -= UpdateKillStreakTimer;
        }

        private void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }

        private void UpdateMultiplier(float multiplier)
        {
            multiplierText.text = multiplier.ToString();
        }

        private void UpdateKillStreakTimer(float timeLeft, float timeMax)
        {
            killStreakTimerSlider.value = timeLeft / timeMax;
        }
    }
}

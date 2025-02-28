using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers.UI
{
    public class ScoreUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text multiplierText;
        [SerializeField] private Image killStreakTimerImage;
        
        private void OnEnable()
        {
            ScoreManager.ScoreManager.Instance.OnScoreUpdated += UpdateScore;
            ScoreManager.ScoreManager.Instance.OnMultiplierUpdated += UpdateMultiplier;
            ScoreManager.ScoreManager.Instance.OnKillstreakTimeUpdated += UpdateKillStreakTimer;
        }

        private void OnDisable()
        {
            ScoreManager.ScoreManager.Instance.OnScoreUpdated -= UpdateScore;
            ScoreManager.ScoreManager.Instance.OnMultiplierUpdated -= UpdateMultiplier;
            ScoreManager.ScoreManager.Instance.OnKillstreakTimeUpdated -= UpdateKillStreakTimer;
        }

        private void UpdateScore(int score)
        {
            scoreText.text = $"Score: {score}";
        }

        private void UpdateMultiplier(float multiplier)
        {
            multiplierText.text = $"Multiplier: x{multiplier:F2}";
        }

        private void UpdateKillStreakTimer(float timeLeft, float timeMax)
        {
            killStreakTimerImage.fillAmount = timeLeft / timeMax;
        }
    }
}

using _Scripts.Managers.Game_Manager_Logic;
using TMPro;
using UnityEngine;

namespace _Scripts.Managers.UI_Logic
{
    public class CoinsUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinText; 
        [SerializeField] private TMP_Text inGameCoinText;

        private void Start()
        {
            UpdateCoinText(GameManager.Instance.PlayerData.playerCoins);
            InGameCoinText(0);
            GameManager.Instance.OnCoinsUpdated += UpdateCoinText;
            GameManager.Instance.OnCoinsUpdated += InGameCoinText;
        }

        private void UpdateCoinText(int currentCoins)
        {
            coinText.text = GameManager.Instance.PlayerData.playerCoins.ToString();
        }

        private void InGameCoinText(int currentCoins)
        {
            inGameCoinText.text = GameManager.Instance.CoinsCollectedThisGame.ToString();
        }
        
        private void OnDestroy()
        {
            GameManager.Instance.OnCoinsUpdated -= UpdateCoinText;
            GameManager.Instance.OnCoinsUpdated -= InGameCoinText;
        }
    }
}

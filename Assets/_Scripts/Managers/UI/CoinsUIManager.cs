using TMPro;
using UnityEngine;

namespace _Scripts.Managers.UI
{
    public class CoinsUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinText; 

        private void Start()
        {
            UpdateCoinText(GameManager.GameManager.Instance.PlayerData.playerCoins);
            GameManager.GameManager.Instance.OnCoinsUpdated += UpdateCoinText;
        }

        private void UpdateCoinText(int currentCoins)
        {
            coinText.text = $"Coins: {GameManager.GameManager.Instance.PlayerData.playerCoins}";
        }

        private void OnDestroy()
        {
            GameManager.GameManager.Instance.OnCoinsUpdated -= UpdateCoinText;
        }
    }
}

using _Scripts.Managers.Game_Manager_Logic;
using TMPro;
using UnityEngine;

namespace _Scripts.Managers.UI_Logic
{
    public class CoinsUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinText; 

        private void Start()
        {
            UpdateCoinText(GameManager.Instance.PlayerData.playerCoins);
            GameManager.Instance.OnCoinsUpdated += UpdateCoinText;
        }

        private void UpdateCoinText(int currentCoins)
        {
            coinText.text = GameManager.Instance.PlayerData.playerCoins.ToString();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnCoinsUpdated -= UpdateCoinText;
        }
    }
}

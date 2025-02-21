using UnityEngine;
using TMPro;
using _Scripts.Managers.GameManager;

namespace _Scripts.Upgrades
{
    public class HealthUpgradeSystem : MonoBehaviour
    {
        [SerializeField] private int healthIncreaseAmount = 1; // Increase per upgrade
        [SerializeField] private int baseUpgradeCost = 10; // Base cost
        [SerializeField] private TMP_Text upgradeCostText; // UI Text for cost

        private int UpgradeLevel
        {
            get => GameManager.Instance.PlayerData.healthUpgradeLevel;
            set
            {
                GameManager.Instance.PlayerData.healthUpgradeLevel = value;
                GameManager.Instance.SaveGameData(); //Save immediately after upgrade
            }
        }

        private void Start()
        {
            UpdateUpgradeUI(GameManager.Instance.PlayerData.playerCoins);
            GameManager.Instance.OnCoinsUpdated += UpdateUpgradeUI;
        }

        public void UpgradeHealth()
        {
            int upgradeCost = CalculateUpgradeCost();
            if (GameManager.Instance.TrySpendCoins(upgradeCost))
            {
                GameManager.Instance.UpgradeMaxHealth(healthIncreaseAmount);
                UpgradeLevel++; //Properly increase upgrade level and save it!
                
                Debug.Log($"Health Upgraded! New Level: {UpgradeLevel}, Max Health: {GameManager.Instance.PlayerData.maxHealth}");

                UpdateUpgradeUI(GameManager.Instance.PlayerData.playerCoins);
            }
        }

        private int CalculateUpgradeCost()
        {
            return baseUpgradeCost * (UpgradeLevel + 1); // Cost increases per level
        }

        private void UpdateUpgradeUI(int currentCoins)
        {
            var upgradeCost = CalculateUpgradeCost();
            upgradeCostText.text = upgradeCost.ToString();
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnCoinsUpdated -= UpdateUpgradeUI;
        }
    }
}
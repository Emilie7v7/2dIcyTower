using _Scripts.CoreSystem;
using _Scripts.CoreSystem.StatSystem;
using _Scripts.Managers.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Upgrades
{
    public class HealthUpgradeSystem : MonoBehaviour
    {
        [SerializeField] private int healthIncreaseAmount = 1; // Increase per upgrade
        [SerializeField] private int upgradeCost = 10; // Example currency cost
        [SerializeField] private int playerCoins; // Example currency
        
        [SerializeField] private Stats playerStats;
        [SerializeField] private HealthUIManager healthUIManager;

        public void UpgradeHealth()
        {
            if (playerStats == null) return;
    
            if (playerCoins >= upgradeCost)
            {
                playerCoins -= upgradeCost;

                // Modify the existing Stat object instead of replacing it
                playerStats.Health.MaxValue += healthIncreaseAmount;
                playerStats.Health.Initialize(); // Reset health to new max

                if (healthUIManager != null)
                {
                    healthUIManager.IncreaseMaxHealth(playerStats.Health.MaxValue);
                    healthUIManager.UpdateHealth(playerStats.Health.CurrentValue); // Force refresh
                }
            }
        }

        public void AddCoins(int amount)
        {
            playerCoins += amount;
        }
    }
}

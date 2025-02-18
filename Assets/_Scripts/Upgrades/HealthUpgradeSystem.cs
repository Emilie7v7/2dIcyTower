using _Scripts.CoreSystem;
using _Scripts.Managers.GameManager;
using _Scripts.Managers.UI;
using UnityEngine;

namespace _Scripts.Upgrades
{
    public class HealthUpgradeSystem : MonoBehaviour
    {
        [SerializeField] private int healthIncreaseAmount = 1; // Increase per upgrade
        [SerializeField] private int upgradeCost = 10; // Example currency cost
        [SerializeField] private int playerCoins; // Example currency

        private void Start()
        {
            Debug.Log("HealthUpgradeSystem Loaded: Max Health = " + GameManager.Instance.MaxHealth);
            GameManager.Instance.AddCoins(playerCoins);
        }

        public void UpgradeHealth()
        {
            if (GameManager.Instance.TrySpendCoins(upgradeCost))
            {
                GameManager.Instance.UpgradeMaxHealth(healthIncreaseAmount);
                Debug.Log("Upgraded Health! New Max Health: " + GameManager.Instance.MaxHealth);
            }
        }
    }
}

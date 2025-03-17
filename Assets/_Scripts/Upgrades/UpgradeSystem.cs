using _Scripts.Managers.Game_Manager_Logic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace _Scripts.Upgrades
{
    public class UpgradeSystem : MonoBehaviour
    {
        private static readonly int NotEnoughCoins = Animator.StringToHash("notEnoughCoins");
        [SerializeField] private string upgradeType; // Example: "Health", "Magnet", "Speed"
        [SerializeField] private int increaseAmount = 1; // Increase per upgrade
        [SerializeField] private int baseUpgradeCost = 10; // Base cost
        [SerializeField] private int maxUpgradeLevel = 7; // Max level for this upgrade

        [SerializeField] private TMP_Text upgradeCostText; // UI Text for cost
        [SerializeField] private Slider upgradeLevelSlider; // UI Slider for upgrade level
        [SerializeField] private Button upgradeButton; // UI Button for upgrading
        [SerializeField] private GameObject warningTextTest; // Warning message

        private void Start()
        {
            //Ensure upgrade exists in dictionary
            GameManager.Instance.PlayerData.UpgradeLevels.TryAdd(upgradeType, 0);

            upgradeLevelSlider.maxValue = maxUpgradeLevel;
            upgradeLevelSlider.value = GameManager.Instance.PlayerData.UpgradeLevels[upgradeType];

            UpdateUpgradeUI(GameManager.Instance.PlayerData.playerCoins);
    
            //Subscribe using a named method (no anonymous lambda)
            GameManager.Instance.OnCoinsUpdated += UpdateUpgradeUI;
        }

        public void UpgradeStat()
        {
            GameManager.Instance.PlayerData.UpgradeLevels.TryAdd(upgradeType, 0);

            var upgradeLevel = GameManager.Instance.PlayerData.UpgradeLevels[upgradeType];

            if (upgradeLevel >= maxUpgradeLevel)
            {
                return;
            }

            var upgradeCost = CalculateUpgradeCost();

            if (GameManager.Instance.PlayerData.playerCoins < upgradeCost)
            {
                ShowWarning();
                return;
            }

            if (GameManager.Instance.TrySpendCoins(upgradeCost))
            {
                GameManager.Instance.PlayerData.UpgradeLevels[upgradeType]++;
                ApplyUpgradeEffect(); //Apply the effect dynamically
                GameManager.Instance.SavePlayerGameData();

                Debug.Log($"{upgradeType} Upgraded! New Level: {GameManager.Instance.PlayerData.UpgradeLevels[upgradeType]}");

                UpdateUpgradeUI(GameManager.Instance.PlayerData.playerCoins);
            }
        }

        private int CalculateUpgradeCost()
        {
            return baseUpgradeCost * (GameManager.Instance.PlayerData.UpgradeLevels[upgradeType] + 1);
        }

        private void UpdateUpgradeUI(int currentCurrency)
        {
            GameManager.Instance.PlayerData.UpgradeLevels.TryAdd(upgradeType, 0);

            var upgradeLevel = GameManager.Instance.PlayerData.UpgradeLevels[upgradeType];

            // Update slider value
            upgradeLevelSlider.value = upgradeLevel;

            // Check if upgrade is maxed out
            if (upgradeLevel >= maxUpgradeLevel)
            {
                upgradeCostText.text = "Max";
            }
            else
            {
                var upgradeCost = CalculateUpgradeCost();
                upgradeCostText.text = upgradeCost.ToString();
            }

            upgradeButton.interactable = true; // Always interactable
        }

        private void ApplyUpgradeEffect()
        {
            switch (upgradeType)
            {
                case "Health":
                    GameManager.Instance.UpgradeMaxHealth(increaseAmount); // Increases max health
                    break;
                
                case "ExplosionRadius":
                    GameManager.Instance.UpgradeMaxExplosionRadius(increaseAmount); // Explosion radius upgrade
                    break;
            
                case "Magnet":
                    GameManager.Instance.UpgradeMagnetDuration(increaseAmount); // Magnet duration upgrade
                    break;
                
                case "Killstreak":
                    GameManager.Instance.UpgradeKillStreakMultiplier(increaseAmount);// Killstreak multiplier upgrade
                    break;
                
                case "RocketBoost":
                    GameManager.Instance.UpgradeRocketDuration(increaseAmount); // Rocket boost duration upgrade
                    break;
                
                case "Immortality":
                    GameManager.Instance.UpgradeImmortalityDuration(increaseAmount); // Immortality duration upgrade
                    break;
            
                default:
                    Debug.LogWarning($"No effect defined for upgrade type: {upgradeType}");
                    break;
            }
        }
        private void ShowWarning()
        {
            var coinAnimator = warningTextTest.gameObject.GetComponent<Animator>();
            coinAnimator.SetBool(NotEnoughCoins, true);
        }

        private void OnDestroy()
        {
            //Properly unsubscribe using the same method reference
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCoinsUpdated -= UpdateUpgradeUI;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using _Scripts.JSON;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public PlayerData PlayerData { get; private set; }

        public event Action<int> OnCoinsUpdated;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadGameData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddCoins(int amount)
        {
            PlayerData.playerCoins += amount;
            OnCoinsUpdated?.Invoke(PlayerData.playerCoins);
            SaveGameData();
        }

        public bool TrySpendCoins(int cost)
        {
            if (PlayerData.playerCoins >= cost)
            {
                PlayerData.playerCoins -= cost;
                OnCoinsUpdated?.Invoke(PlayerData.playerCoins);
                SaveGameData();
                return true;
            }
            return false;
        }

        public void UpgradeMaxHealth(int amount)
        {
            if (PlayerData.maxHealth < 10)
            {
                PlayerData.maxHealth += amount;
                SaveGameData();
                Debug.Log($"Max Health Upgraded: {PlayerData.maxHealth}/10");
            }
            else
            {
                Debug.Log("Max Health already at the limit");
            }
        }

        public void UpgradeMaxExplosionRadius(int amount)
        {
            if (PlayerData.explosionRadiusBonus < 10)
            {
                PlayerData.explosionRadiusBonus += amount;
                SaveGameData();
                Debug.Log($"Max Explosion Radius Upgraded: {PlayerData.explosionRadiusBonus}/10");
            }
            else
            {
                Debug.Log("Max Radius already at the limit");
            }
        }

        public void UpgradeMagnetDuration(int amount)
        {
            if (PlayerData.magnetDuration < 10)
            {
                PlayerData.magnetDuration += amount;
                SaveGameData();
                Debug.Log($"Magnet Duration Upgraded: {PlayerData.magnetDuration}/10");
            }
            else
            {
                Debug.Log("Magnet Duration already at the limit");
            }
        }
        
        #region Game Data
        
        public void SaveGameData()
        {
            SaveSystem.SaveData(PlayerData);
        }

        private void LoadGameData()
        {
            if (File.Exists(SaveSystem.SavePath))
            {
                PlayerData = SaveSystem.LoadData();
            }
            else
            {
                PlayerData = new PlayerData();
                SaveGameData();
            }

            // Ensure the upgrade dictionary exists
            if (PlayerData.UpgradeLevels == null)
            {
                PlayerData.UpgradeLevels = new Dictionary<string, int>();
            }

            // List of all upgrade types (Add new types here when needed)
            var defaultUpgrades = new List<string> {"Health", "Magnet", "Explosion", "Multiplier", "Rocket", "Immortality"};

            // Ensure all upgrades exist in the dictionary
            foreach (var upgrade in defaultUpgrades)
            {
                if (!PlayerData.UpgradeLevels.ContainsKey(upgrade))
                {
                    PlayerData.UpgradeLevels[upgrade] = 0; // Set to default level
                }
            }

            OnCoinsUpdated?.Invoke(PlayerData.playerCoins);
        }
        
        public void ResetGameData()
        {
            SaveSystem.DeleteData(); // Step 1: Delete the save file
            PlayerData = new PlayerData(); // Step 2: Create brand-new PlayerData
            
            Debug.Log($"New PlayerData created: Coins = {PlayerData.playerCoins}, Max Health = {PlayerData.maxHealth}, Upgrade Level = {PlayerData.UpgradeLevels.ContainsKey("Health")}");

            SaveGameData(); // Step 3: Save fresh new data
            LoadGameData(); // Step 4: Ensure we are using the fresh save

            OnCoinsUpdated?.Invoke(PlayerData.playerCoins); // Step 5: Update UI
            
        }
        #endregion
    }
}

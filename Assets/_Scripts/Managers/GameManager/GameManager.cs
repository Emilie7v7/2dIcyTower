using System;
using System.IO;
using _Scripts.JSON;
using UnityEngine;
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
                Debug.Log("🚨 No Save File Found. Creating new PlayerData.");
                PlayerData = new PlayerData();
                SaveGameData();
            }
        }
        
        public void ResetGameData()
        {
            Debug.Log("🚨 RESETTING GAME DATA 🚨");

            SaveSystem.DeleteData(); // Step 1: Delete the save file
            Debug.Log("✅ Save file deleted.");

            PlayerData = new PlayerData(); // Step 2: Create brand-new PlayerData
            Debug.Log($"✅ New PlayerData created: Coins = {PlayerData.playerCoins}, Max Health = {PlayerData.maxHealth}, Upgrade Level = {PlayerData.healthUpgradeLevel}");

            SaveGameData(); // Step 3: Save fresh new data
            Debug.Log("✅ New data saved.");

            LoadGameData(); // Step 4: Ensure we are using the fresh save
            Debug.Log($"✅ Data after reload: Coins = {PlayerData.playerCoins}, Max Health = {PlayerData.maxHealth}, Upgrade Level = {PlayerData.healthUpgradeLevel}");

            OnCoinsUpdated?.Invoke(PlayerData.playerCoins); // Step 5: Update UI
            Debug.Log("🔥 Game Data Reset Successfully!");

            // Step 6: Reload the scene to clear all lingering references
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}

using System;
using _Scripts.JSON;
using UnityEngine;

namespace _Scripts.Managers.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public PlayerData PlayerData { get; private set; }
        
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
            SaveGameData();
        }

        public bool TrySpendCoins(int cost)
        {
            if (PlayerData.playerCoins >= cost)
            {
                PlayerData.playerCoins -= cost;
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
                Debug.Log($"Max Health already at the limit");
            }
        }

        private void SaveGameData()
        {
            SaveSystem.SaveData(PlayerData);
        }

        private void LoadGameData()
        {
            PlayerData = SaveSystem.LoadData();
        }
        
        //For testing purposes will be either deleted later or kept as a setting for player to choose if they wanna reset their upgrades etc
        public void ResetGameData()
        {
            SaveSystem.DeleteData();
            PlayerData = new PlayerData();
            SaveGameData();
            
            Debug.Log("Game Data Reset! Coins: " + PlayerData.playerCoins + ", Max Health: " + PlayerData.maxHealth);
        }
    }
}

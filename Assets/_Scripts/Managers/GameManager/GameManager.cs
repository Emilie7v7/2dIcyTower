using System;
using _Scripts.JSON;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                Debug.Log("Max Health already at the limit");
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
        
        public void ResetGameData()
        {
            SaveSystem.DeleteData();
            LoadGameData();
            
            Debug.Log("Game Data Reset! Coins: " + PlayerData.playerCoins + ", Max Health: " + PlayerData.maxHealth);
        }

    }
}

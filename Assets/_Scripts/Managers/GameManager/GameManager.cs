using System;
using UnityEngine;

namespace _Scripts.Managers.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public int PlayerCoins { get; private set; }
        public int MaxHealth { get; private set; } = 3;

        private const int MaxHealthLimit = 10;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
        
                LoadGameData(); //Ensure data is loaded IMMEDIATELY on Awake
        
                // Ensure minimum health is never 0
                if (MaxHealth <= 0)
                {
                    MaxHealth = 3; // Default starting health
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddCoins(int amount)
        {
            PlayerCoins += amount;
            SaveGameData();
        }

        public bool TrySpendCoins(int cost)
        {
            if (PlayerCoins >= cost)
            {
                PlayerCoins -= cost;
                SaveGameData();
                return true;
            }
            return false;
        }

        public void UpgradeMaxHealth(int amount)
        {
            if (MaxHealth < MaxHealthLimit)
            {
                MaxHealth = Mathf.Min(MaxHealth + amount, MaxHealthLimit);
                SaveGameData();
                Debug.Log($"Max Health Upgraded: {MaxHealth}/{MaxHealthLimit}");
            }
            else
            {
                Debug.Log($"Max Health already at the limit");
            }
        }

        private void SaveGameData()
        {
            PlayerPrefs.SetInt("MaxHealth", MaxHealth);
            PlayerPrefs.SetInt("PlayerCoins", PlayerCoins);
            PlayerPrefs.Save();
        }

        private void LoadGameData()
        {
            MaxHealth = PlayerPrefs.GetInt("MaxHealth");
            PlayerCoins = PlayerPrefs.GetInt("PlayerCoins");
        }
        
        //For testing purposes will be either deleted later or kept as a setting for player to choose if they wanna reset their upgrades etc
        public void ResetGameData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
            //Reset in memory values immediately
            PlayerCoins = 10;
            MaxHealth = 3;
            
            SaveGameData();
            
            Debug.Log("Game Data Reset! Coins: " + PlayerCoins + ", Max Health: " + MaxHealth);
        }
    }
}

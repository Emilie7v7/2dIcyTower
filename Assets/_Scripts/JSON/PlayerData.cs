using System;
using UnityEngine.Serialization;

namespace _Scripts.JSON
{
    [Serializable]
    public class PlayerData
    {
        public int playerCoins;
        public int maxHealth;
        public int healthUpgradeLevel;

        public PlayerData()
        {
            playerCoins = 0;
            maxHealth = 3;
            healthUpgradeLevel = 0;
        }
    }
}

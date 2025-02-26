using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace _Scripts.JSON
{
    [Serializable]
    public class PlayerData
    {
        public int playerCoins;
        public int maxHealth;
        public int explosionRadiusBonus;
        public int magnetDuration;
        public int killstreakMultiplier;
        public int rocketBoostSpeed;
        public int immortalityDuration;
        public Dictionary<string, int> UpgradeLevels;

        public PlayerData()
        {
            playerCoins = 0;
            maxHealth = 3;
            explosionRadiusBonus = 1;
            magnetDuration = 5;
            UpgradeLevels = new Dictionary<string, int>
            {
                { "Health", 0 },
                { "Magnet", 0 }
            };
        }
    }
}

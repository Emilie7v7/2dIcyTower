using System;
using System.Collections.Generic;

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
        public int rocketBoostDuration;
        public int immortalityDuration;
        public Dictionary<string, int> UpgradeLevels;

        public PlayerData()
        {
            playerCoins = 0;
            maxHealth = 3;
            explosionRadiusBonus = 1;
            magnetDuration = 5;
            rocketBoostDuration = 5;
            immortalityDuration = 10;
            killstreakMultiplier = 2;
            UpgradeLevels = new Dictionary<string, int>
            {
                { "Health", 0 },
                { "Magnet", 0 }
            };
        }
    }
}

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
        public int magnetRange; // New stat
        public Dictionary<string, int> UpgradeLevels;
        public int killstreakMultiplier;
        public int rocketBoostSpeed;
        public int explosionRadius;
        public int immortalityDuration;

        public PlayerData()
        {
            playerCoins = 0;
            maxHealth = 3;
            magnetRange = 0; // Default magnet range
            UpgradeLevels = new Dictionary<string, int>
            {
                { "Health", 0 },
                { "Magnet", 0 }
            };
        }
    }
}

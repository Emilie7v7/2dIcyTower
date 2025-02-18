using System;
using UnityEngine.Serialization;

namespace _Scripts.JSON
{
    [Serializable]
    public class PlayerData
    {
        public int playerCoins;
        public int maxHealth;

        public PlayerData()
        {
            playerCoins = 0;
            maxHealth = 3;
        }
    }
}

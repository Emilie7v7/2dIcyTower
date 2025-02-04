using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ScriptableObjects.PlayerData
{
    [CreateAssetMenu(menuName = "Data/Player Data/Base Data", fileName = ("newPlayerData"))]
    public class PlayerDataSo : ScriptableObject
    {
        [Header("Player Movement Speed")] 
        public float playerMovementSpeed = 5f;
        
        [Header("ProjectilePrefab")]
        public GameObject projectile;
    }
        
}

using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ScriptableObjects.PlayerData
{
    [CreateAssetMenu(menuName = "Data/Player Data/Base Data", fileName = ("newPlayerData"))]
    public class PlayerDataSo : ScriptableObject
    {
        [Header("Player Movement Properties")] 
        public float playerMovementSpeed;
        public float decelerationRate;
        
        [Header("ProjectilePrefab")]
        public GameObject projectilePrefab;

        [Header("Speed Rotation Of Indicator")]
        public float rotationSpeed;

    }
        
}

using UnityEngine;

namespace _Scripts.ScriptableObjects.EntityData
{
    [CreateAssetMenu(menuName = "Data/Entity Data/Base Data", fileName = "newEntityData")]
    public class EntityDataSo : ScriptableObject
    {
        [Header("Movement Speed")] 
        public float movementSpeed;
        
        [Header("Projectile Prefab")]
        public GameObject projectilePrefab;
        
        [Header("Min Idle Time/Max Idle Time")]
        public float minIdleTime;
        public float maxIdleTime;

        [Header("Long Range Attack Cooldown")] 
        public float longRangeAttackCooldown;

    }
}

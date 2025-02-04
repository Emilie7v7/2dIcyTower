using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ScriptableObjects.EntityData
{
    [CreateAssetMenu(menuName = "Data/Entity Data/Base Data", fileName = "newEntityData")]
    public class EntityDataSo : ScriptableObject
    {
        [Header("Movement Speed")] 
        public float movementSpeed;
        
        [Header("Projectile Properties")]
        public GameObject projectilePrefab;
        public float projectileDamage;
        public float projectileSpeed;
        
        [Header("Min Idle Time/Max Idle Time")]
        public float minIdleTime;
        public float maxIdleTime;

        [Header("Long Range Attack Cooldown")] 
        public float longRangeAttackCooldown;

    }
}

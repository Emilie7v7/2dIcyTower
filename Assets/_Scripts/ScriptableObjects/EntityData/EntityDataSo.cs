using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ScriptableObjects.EntityData
{
    [CreateAssetMenu(menuName = "Data/Entity Data/Base Data", fileName = "newEntityData")]
    public class EntityDataSo : ScriptableObject
    {
        [Header("Movement Speed")] 
        public float movementSpeed;
        
        [Header("Projectile Speed")]
        public float projectileSpeed;
        
        [Header("Projectile Damage")]
        public float projectileDamage;
        
        [Header("Min Idle Time/Max Idle Time")]
        public float minIdleTime;
        public float maxIdleTime;
        
    }
}

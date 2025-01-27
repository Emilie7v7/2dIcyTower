using UnityEngine;

namespace _Scripts.ScriptableObjects.EntityData
{
    [CreateAssetMenu(menuName = "Data/Entity Data/Base Data", fileName = "newEntityData")]
    public class EntityDataSo : ScriptableObject
    {
        [Header("Movement Speed")] 
        public float MovementSpeed;
        
        [Header("Projectile Speed")]
        public float ProjectileSpeed;
        
        [Header("Projectile Damage")]
        public float ProjectileDamage;
        
        [Header("Min Idle Time/Max Idle Time")]
        public float MinIdleTime;
        public float MaxIdleTime;
    }
}

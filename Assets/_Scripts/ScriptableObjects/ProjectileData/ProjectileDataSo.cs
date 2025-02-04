using UnityEngine;

namespace _Scripts.ScriptableObjects.ProjectileData
{
    public class ProjectileDataSo : ScriptableObject
    {
        [Header("Projectile Properties")]
        public float projectileSpeed;
        public float projectileDamage;
        public float projectileRadius;
    }
}

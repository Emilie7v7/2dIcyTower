using UnityEngine;

namespace _Scripts.ScriptableObjects.ProjectileData
{
    [CreateAssetMenu(menuName = "Data/Projectile Data/Base Data", fileName = "newProjectileData")]
    public class ProjectileDataSo : ScriptableObject
    {
        [Header("Projectile Properties")]
        public float projectileSpeed;
        public float projectileDamage;
        public float projectileRadius;
        public Vector2 projectileArc;
    }
}

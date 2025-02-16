using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ScriptableObjects.ProjectileData
{
    [CreateAssetMenu(menuName = "Data/Projectile Data/Base Data", fileName = "newProjectileData")]
    public class ProjectileDataSo : ScriptableObject
    {
        [Header("Projectile Properties")]
        public GameObject explosionPrefab;
        public float projectileSpeed;
        public float projectileRadius;
        public Vector2 projectileArc;
        public int maxHitsRayForProjectile;
        public float maxTravelDistance;
        
        [Header("Projectile Target")]
        public LayerMask targetLayer;
    }
}

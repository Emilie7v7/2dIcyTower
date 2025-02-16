using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ScriptableObjects.ExplosionData
{
    [CreateAssetMenu(menuName = "Data/Explosion Data/Base Data", fileName = ("newExplosionData"))]
    public class ExplosionDataSo : ScriptableObject
    {
        [Header("Explosion Properties")] 
        public float explosionRadius;
        public Vector2 explosionStrength;
        public int maxHitsRayForExplosion;
        public int explosionDamage;
        
        [Header("Explosion Target")]
        public LayerMask targetLayer;
        
        [Header("Self Layer For No Damage")]
        public LayerMask noDamageLayer;
    }
}

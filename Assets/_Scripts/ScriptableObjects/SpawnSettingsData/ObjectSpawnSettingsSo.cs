using UnityEngine;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    public enum ObjectType
    {
        Enemy,      // Must spawn on a platform
        PowerUp,    // Can spawn in the air
        Coin        // Can spawn in the air
    }

    [CreateAssetMenu(menuName = "Data/Object Spawn Data/Base Data", fileName = "newObjectSpawnData")]
    public class ObjectSpawnSettingsSo : ScriptableObject
    {
        [Header("Object Spawn Settings")]
        public GameObject[] objectPrefabs;
        [HideInInspector] public ObjectType objectType; // NEW: Defines the type of object
        public float spawnProbability = 0.5f;
        public int maxObjectsPerChunk = 3;
        public float spawnCooldown = 2f;
        public Vector2 spawnHeightRange = new Vector2(0, 10000);
    }
}

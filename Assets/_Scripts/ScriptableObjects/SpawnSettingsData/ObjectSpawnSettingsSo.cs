using UnityEngine;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Object Spawn Data/Base Data", fileName = "newObjectSpawnData")]
    public class ObjectSpawnSettingsSo : ScriptableObject
    {
        [Header("Object Spawn Settings")]
        public GameObject[] objectPrefabs;
        public float spawnProbability = 0.5f; // Default 50% chance
        public bool mustSpawnOnPlatform;
        public bool canSpawnMidAir;
        public int maxObjectsPerChunk = 3; // Max objects per chunk
        public float spawnCooldown = 2f; // Default cooldown between spawns
        public Vector2 spawnHeightRange = new Vector2(0, 10000); // Restrict object spawn height
    }
}

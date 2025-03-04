using UnityEngine;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Object Spawn Data/Base Data", fileName = "newObjectSpawnData")]
    public class ObjectSpawnSettingsSo : ScriptableObject
    {
        [Header("Object Spawn Settings")]
        public GameObject[] coinsPrefab;
        public int coinsPerChunk;
        public GameObject[] enemiesPrefab;
        public int minEnemiesPerChunk = 1;
        public int maxEnemiesPerChunk = 3;
        public GameObject[] boostsPrefab;
        public float dropBoostsChanceFromEnemy = 0.5f;
        public bool mustSpawnOnPlatform = false;
        public bool canSpawnAnywhere = false;
        public Vector2 spawnHeightRange = new Vector2(0, 10000);
    }
}

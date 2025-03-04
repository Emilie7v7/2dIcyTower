using UnityEngine;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Object Spawn Data/Base Data", fileName = "newObjectSpawnData")]
    public class ObjectSpawnSettingsSo : ScriptableObject
    {
        public GameObject[] coinsPrefab;
        
        [HideInInspector] public int minCoinsPerChunk = 5;  // Minimum value
        [HideInInspector] public int maxCoinsPerChunk = 10; // Maximum value


        public GameObject[] enemiesPrefab;
        public int minEnemiesPerChunk = 1;
        public int maxEnemiesPerChunk = 3;
        
        public GameObject[] boostsPrefab;
        public float dropBoostsChanceFromEnemy = 0.5f;
        
        public bool mustSpawnOnPlatform = false;
        public bool canSpawnAnywhere = false;
        
        public Vector2 spawnHeightRange = new Vector2(0, 10000);
        
        public int GetRandomCoinsPerChunk()
        {
            return Random.Range(minCoinsPerChunk, maxCoinsPerChunk + 1); // Inclusive max
        }
    }
}

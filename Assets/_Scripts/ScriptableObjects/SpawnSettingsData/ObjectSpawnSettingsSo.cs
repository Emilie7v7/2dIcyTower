using _Scripts.Attributes;
using UnityEngine;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Object Spawn Data/Base Data", fileName = "newObjectSpawnData")]
    public class ObjectSpawnSettingsSo : ScriptableObject
    {
        [HideInInspector] public GameObject[] coinsPrefab;
        
        [HideInInspector] public int minCoinsPerChunk = 5;  // Minimum value
        [HideInInspector] public int maxCoinsPerChunk = 10; // Maximum value

        [HideInInspector] public GameObject[] enemiesPrefab;
        [HideInInspector] public int minEnemiesPerChunk = 1;
        [HideInInspector] public int maxEnemiesPerChunk = 3;
        
        [HideInInspector] public GameObject[] boostsPrefab;
        [HideInInspector] public float dropBoostsChanceFromEnemy = 0.5f;
        
        [HideInInspector] public bool mustSpawnOnPlatform = false;
        [HideInInspector] public bool canSpawnAnywhere = false;
        
        [HideInInspector] public Vector2 spawnHeightRange = new Vector2(0, 10000);
        
        public int GetRandomCoinsPerChunk()
        {
            return Random.Range(minCoinsPerChunk, maxCoinsPerChunk + 1); // Inclusive max
        }
    }
}

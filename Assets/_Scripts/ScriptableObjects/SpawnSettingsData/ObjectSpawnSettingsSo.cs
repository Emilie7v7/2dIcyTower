using _Scripts.Attributes;
using UnityEngine;

namespace _Scripts.ScriptableObjects.SpawnSettingsData
{
    [CreateAssetMenu(menuName = "Data/Object Spawn Data/Base Data", fileName = "newObjectSpawnData")]
    public class ObjectSpawnSettingsSo : ScriptableObject
    {
        [HideInInspector] public GameObject[] coinsPrefab;

        [HideInInspector] public int minCoinsPerChunk;
        [HideInInspector] public int maxCoinsPerChunk;

        [HideInInspector] public GameObject[] enemiesPrefab;
        [HideInInspector] public int minEnemiesPerChunk;
        [HideInInspector] public int maxEnemiesPerChunk;
        
        public BoostInfo[] boosts;
        
        [HideInInspector] public bool mustSpawnOnPlatform = false;
        [HideInInspector] public bool canSpawnAnywhere = false;
        
        [HideInInspector] public Vector2 spawnHeightRange = new Vector2(0, 10000);
        
        public int GetRandomCoinsPerChunk()
        {
            return Random.Range(minCoinsPerChunk, maxCoinsPerChunk + 1);
        }
        public int GetRandomEnemiesPerChunk()
        {
            return Random.Range(minEnemiesPerChunk, maxEnemiesPerChunk + 1);
        }
    }

    [System.Serializable]
    public class BoostInfo
    {
        public string boostName;
        [Range(0,100)]
        public float dropChance;
        public GameObject boostPrefab;
    }
}

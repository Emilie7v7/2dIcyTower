using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace _Scripts.ChunkGeneration.Spawner
{
    public enum ChunkType
    {
        Spawn,
        Easy,
        Medium,
        Hard,
        Bonus
    }

    [System.Serializable]
    public class ChunkPrefabGroup
    {
        public ChunkType type;
        public List<GameObject> prefabs;
    }

    public class ChunkSpawner : MonoBehaviour
    {
        [Header("Chunk Settings")]
        [SerializeField] private float chunkHeight = 100f;
        [SerializeField] private List<ChunkPrefabGroup> chunkPrefabs;

        [Header("Spawn Heights")]
        [SerializeField] private float mediumChunksStartHeight = 1000f;
        [SerializeField] private float hardChunksStartHeight = 5000f;

        [Header("Bonus Chunk Settings")]
        [SerializeField] [Range(0f, 1f)] private float bonusChunkSpawnChance = 0.05f;

        private Dictionary<ChunkType, List<GameObject>> chunkPools; // Changed from Queue to List for random selection
        private List<GameObject> activeChunks;
        private Transform playerTransform;
        private float nextSpawnHeight;

        private void Start()
        {
            InitializeChunkPools();
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            activeChunks = new List<GameObject>();
            
            // Spawn initial chunks
            SpawnInitialChunks();
        }

        private void Update()
        {
            CheckAndSpawnChunks();
            CleanupOldChunks();
        }

        private void InitializeChunkPools()
        {
            chunkPools = new Dictionary<ChunkType, List<GameObject>>();
            
            var poolContainer = new GameObject("Chunk Pools").transform;
            poolContainer.parent = transform;
            
            // Initialize pool for each chunk type
            foreach (var prefabGroup in chunkPrefabs)
            {
                chunkPools[prefabGroup.type] = new List<GameObject>();
        
                // Instantiate all prefabs in the group
                foreach (var prefab in prefabGroup.prefabs)
                {
                    var chunk = Instantiate(prefab, poolContainer);
                    chunk.SetActive(false);
                    chunkPools[prefabGroup.type].Add(chunk);
                }
            }
        }

        private void SpawnInitialChunks()
        {
            // Spawn one spawn chunk
            SpawnChunk(ChunkType.Spawn, Vector3.zero);

            // Spawn two easy chunks
            for (var i = 0; i < 2; i++)
            {
                SpawnChunk(ChunkType.Easy, Vector3.up * (chunkHeight * (i + 1)));
            }

            nextSpawnHeight = chunkHeight * 3;
        }

        private void CheckAndSpawnChunks()
        {
            if (playerTransform.position.y + chunkHeight > nextSpawnHeight)
            {
                var typeToSpawn = DetermineChunkType(nextSpawnHeight);
                SpawnChunk(typeToSpawn, Vector3.up * nextSpawnHeight);
                nextSpawnHeight += chunkHeight;
            }
        }

        private ChunkType DetermineChunkType(float height)
        {
            var random = Random.value;

            // Try spawn bonus chunk first
            if (Random.value < bonusChunkSpawnChance)
                return ChunkType.Bonus;

            // Above hard chunks threshold
            if (height >= hardChunksStartHeight)
            {
                if (random < 0.15f) return ChunkType.Easy;
                if (random < 0.50f) return ChunkType.Medium;
                return ChunkType.Hard;
            }
            // Above medium chunks threshold
            else if (height >= mediumChunksStartHeight)
            {
                return random < 0.5f ? ChunkType.Easy : ChunkType.Medium;
            }
            // Below all thresholds - only easy chunks
            else
            {
                return ChunkType.Easy;
            }
        }

        private void SpawnChunk(ChunkType type, Vector3 position)
        {
            var availableChunks = chunkPools[type].Where(c => !c.activeInHierarchy).ToList();
    
            if (availableChunks.Count == 0)
            {
                Debug.LogWarning($"No inactive chunks of type {type} available. Consider increasing pool size.");
                return;
            }

            // Get random inactive chunk from pool
            var randomIndex = Random.Range(0, availableChunks.Count);
            var chunk = availableChunks[randomIndex];
    
            chunk.transform.position = position;
            chunk.SetActive(true);
            activeChunks.Add(chunk);
        }

        private void CleanupOldChunks()
        {
            for (var i = activeChunks.Count - 1; i >= 0; i--)
            {
                if (activeChunks[i].transform.position.y < playerTransform.position.y - chunkHeight)
                {
                    var chunk = activeChunks[i];
                    chunk.SetActive(false);
                    activeChunks.RemoveAt(i);
                    // No need to add back to pool since we're just enabling/disabling now
                }
            }
        }

        private static ChunkType DetermineChunkTypeFromPrefab(GameObject chunk)
        {
            if (chunk.CompareTag("Spawn")) return ChunkType.Spawn;
            if (chunk.CompareTag("Easy")) return ChunkType.Easy;
            if (chunk.CompareTag("Medium")) return ChunkType.Medium;
            if (chunk.CompareTag("Hard")) return ChunkType.Hard;
            if (chunk.CompareTag("Bonus")) return ChunkType.Bonus;
    
            Debug.LogWarning($"Chunk {chunk.name} has no recognized tag! Defaulting to Easy type.");
            return ChunkType.Easy;
        }

    }
}
using UnityEngine;
using System.Collections.Generic;

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

        private Dictionary<ChunkType, Queue<GameObject>> chunkPools;
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
            chunkPools = new Dictionary<ChunkType, Queue<GameObject>>();
            foreach (ChunkType type in System.Enum.GetValues(typeof(ChunkType)))
            {
                chunkPools[type] = new Queue<GameObject>();
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
            GameObject chunk;
            if (chunkPools[type].Count > 0)
            {
                chunk = chunkPools[type].Dequeue();
                chunk.SetActive(true);
            }
            else
            {
                var prefabGroup = chunkPrefabs.Find(x => x.type == type);
                var prefab = prefabGroup.prefabs[Random.Range(0, prefabGroup.prefabs.Count)];
                chunk = Instantiate(prefab);
            }

            chunk.transform.position = position;
            activeChunks.Add(chunk);
        }

        private void CleanupOldChunks()
        {
            for (var i = activeChunks.Count - 1; i >= 0; i--)
            {
                if (activeChunks[i].transform.position.y < playerTransform.position.y - chunkHeight)
                {
                    var chunk = activeChunks[i];
                    var type = DetermineChunkTypeFromPrefab(chunk);
                    chunk.SetActive(false);
                    chunkPools[type].Enqueue(chunk);
                    activeChunks.RemoveAt(i);
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
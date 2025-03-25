using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Managers.Chunk_Logic
{
    public class ChunkSpawner : MonoBehaviour
    {
        public Transform player;
        public GameObject[] chunkPrefabs;
        public GameObject[] enemyPrefabs, coinPrefabs, decorationPrefabs;

        private float _nextSpawnY = 0f;
        private const float ChunkHeight = 30f;
        private readonly Queue<GameObject> _spawnedChunks = new();

        private void Update()
        {
            if (player.position.y + (ChunkHeight * 2) > _nextSpawnY)
            {
                SpawnNextChunk();
            }
        }

        private void SpawnNextChunk()
        {
            var prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
            var chunk = Instantiate(prefab, new Vector3(0, _nextSpawnY, 0), Quaternion.identity);

            var controller = chunk.GetComponent<ChunkController>();
            controller.SpawnContents(enemyPrefabs, coinPrefabs, decorationPrefabs);

            _spawnedChunks.Enqueue(chunk);
            _nextSpawnY += ChunkHeight;

            if (_spawnedChunks.Count > 5)
            {
                var oldChunk = _spawnedChunks.Dequeue();
                Destroy(oldChunk);
            }
        }
    }
}
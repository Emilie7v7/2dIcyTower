using UnityEngine;

namespace _Scripts.Managers.Chunk_Logic
{
    public class ChunkController : MonoBehaviour
    {
        [SerializeField] private Transform[] enemySpawnPoints;
        [SerializeField] private Transform[] coinSpawnPoints;
        [SerializeField] private Transform[] decorationSpawnPoints;

        public void SpawnContents(GameObject[] enemyPrefabs, GameObject[] coinPrefabs, GameObject[] decorationPrefabs)
        {
            SpawnFromPool(enemySpawnPoints, enemyPrefabs);
            SpawnFromPool(coinSpawnPoints, coinPrefabs);
            SpawnFromPool(decorationSpawnPoints, decorationPrefabs);
        }

        private void SpawnFromPool(Transform[] points, GameObject[] prefabs)
        {
            foreach (var point in points)
            {
                if (Random.value < 0.6f)
                {
                    var prefab = prefabs[Random.Range(0, prefabs.Length)];
                    Instantiate(prefab, point.position, Quaternion.identity, this.transform);
                }
            }
        }
    }
}
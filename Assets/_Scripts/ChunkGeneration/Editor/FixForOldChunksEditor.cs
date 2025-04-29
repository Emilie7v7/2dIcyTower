using UnityEditor;
using UnityEngine;

namespace _Scripts.ChunkGeneration.Editor
{
    public class FixForOldChunksEditor : EditorWindow
    {
        [SerializeField] private GameObject coinsPrefab;
        [SerializeField] private GameObject enemiesPrefab;
        [SerializeField] private GameObject chunkPrefab;
        
        [MenuItem("Fix/Fix for old chunks")]
        public static void ShowWindow()
        {
            GetWindow<FixForOldChunksEditor>("Fix Generator");
        }

        private void OnGUI()
        {
            coinsPrefab = (GameObject) EditorGUILayout.ObjectField("Coins", coinsPrefab, typeof(GameObject), false);
            enemiesPrefab = (GameObject) EditorGUILayout.ObjectField("Enemies", enemiesPrefab, typeof(GameObject), false);
            chunkPrefab = (GameObject) EditorGUILayout.ObjectField("Chunk", chunkPrefab, typeof(GameObject), true);

            if (GUILayout.Button("FixCoins"))
            {
                FixCoins();
            }

            if (GUILayout.Button("FixEnemies"))
            {
                FixEnemies();
            }
        }

        private void FixCoins()
        {
            // Setup parent for coins and remove the old ones for regeneration of the chunk
            var spawnPoint = chunkPrefab;
            var spawnPointParent = spawnPoint.transform.Find("SpawnPoints");
            var coinsParent = spawnPointParent.transform.Find("Coins");

            foreach (Transform coin in coinsParent.transform)
            {
                Instantiate(coinsPrefab, coin.position, Quaternion.identity);
            }
        }

        private void FixEnemies()
        {
            var spawnPoint = chunkPrefab;
            var spawnPointParent = spawnPoint.transform.Find("SpawnPoints");
            var enemiesParent = spawnPointParent.transform.Find("Enemies");

            foreach (Transform enemy in enemiesParent.transform)
            {
                Instantiate(enemiesPrefab, enemy.position, Quaternion.identity);
            }
        }
    }
}
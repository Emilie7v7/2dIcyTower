using UnityEngine;

namespace _Scripts.ChunkGeneration.Spawner
{
    public class ChunkReset : MonoBehaviour
    {
        private GameObject[] collectibles;
        private GameObject[] enemies;
        private GameObject[] goldenChest;
        private GameObject[] woodenChest;

        private void Awake()
        {
            // Cache all collectibles and enemies when the chunk is first created
            collectibles = GameObject.FindGameObjectsWithTag("Coin");
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            goldenChest = GameObject.FindGameObjectsWithTag("GoldChest");
            woodenChest = GameObject.FindGameObjectsWithTag("WoodChest");
        }

        private void OnEnable()
        {
            // When the chunk becomes active, reactivate all its collectibles and enemies
            foreach (var collectible in collectibles)
            {
                if (collectible.transform.IsChildOf(transform))
                {
                    collectible.SetActive(true);
                }
            }

            foreach (var enemy in enemies)
            {
                if (enemy.transform.IsChildOf(transform))
                {
                    enemy.SetActive(true);
                }
            }
            
            foreach (var goldChest in goldenChest)
            {
                if (goldChest.transform.IsChildOf(transform))
                {
                    goldChest.SetActive(true);
                }
            }
            
            foreach (var woodChest in woodenChest)
            {
                if (woodChest.transform.IsChildOf(transform))
                {
                    woodChest.SetActive(true);
                }
            }
        }
    }
}
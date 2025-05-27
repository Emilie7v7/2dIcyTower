using System.Collections;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.Objects;
using _Scripts.Pickups;
using _Scripts.Projectiles;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers.Menu_Logic
{
    public class MenuManager : MonoBehaviour
    {

        public void StartGame()
        {
            Time.timeScale = 1;
            StartCoroutine(LoadLoadingScreen());
        }

        private IEnumerator LoadLoadingScreen()
        {
            // Start loading the loading screen asynchronously
            var loadOperation = SceneManager.LoadSceneAsync("LoadingScene");
            loadOperation.allowSceneActivation = true;
    
            // Wait for the loading screen to load
            while (!loadOperation.isDone)
            {
                yield return null;
            }
        }


        public void BackToMenu()
        {
            GameManager.Instance.SavePlayerGameData();
            Time.timeScale = 1;
            ResetPooledObjects();
            SceneManager.LoadScene("MenuScene");
        }

        public void UponDeathBackToMenu()
        {
            Time.timeScale = 1;
            ResetPooledObjects();
            SceneManager.LoadScene("MenuScene");
        }
        
        public void RestartGame()
        {
            Time.timeScale = 1;
            ResetPooledObjects();
            StartCoroutine(LoadLoadingScreen());
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }
        
        #region Return Of Unused Objects To Pool
        private static void ResetPooledObjects()
        {
            if (EnemyPool.Instance != null)
            {
                ReturnActiveEnemies();
            }

            if (CoinPool.Instance != null)
            {
                ReturnActiveCoins();
            }

            if (PowerUpPool.Instance != null)
            {
                ReturnActivePowerUps();
            }

            if (ChestCoinPool.Instance != null)
            {
                ReturnActiveChestCoins();
            }

            if (PlayerProjectilePool.Instance && EnemyProjectilePool.Instance != null)
            {
                ReturnActiveProjectiles();
            }

            if (PlayerExplosionPool.Instance && EnemyExplosionPool.Instance != null)
            {
                ReturnActiveExplosion();
            }

            if (GoldenChestPool.Instance && WoodenChestPool.Instance != null)
            {
                ReturnActiveChests();
            }

            if (ArrowPool.Instance != null)
            {
                ReturnActiveArrows();
            }
        }

        // Return all active enemies to the pool
        private static void ReturnActiveEnemies()
        {
            var activeEnemies = GameObject.FindGameObjectsWithTag("Enemy"); // Find all active enemies
            foreach (var enemy in activeEnemies)
            {
                var enemyComponent = enemy.GetComponent<Entity>(); // Ensure it has the entity component
                if (enemyComponent != null)
                {
                    EnemyPool.Instance.ReturnObject(enemyComponent);
                }
            }
        }

        // Return all active coins to the pool
        private static void ReturnActiveCoins()
        {
            var activeCoins = GameObject.FindGameObjectsWithTag("Coin"); // Find all active coins
            foreach (var coin in activeCoins)
            {
                var coinComponent = coin.GetComponent<CoinPickup>(); // Ensure it has the coin component
                if (coinComponent != null)
                {
                    CoinPool.Instance.ReturnObject(coinComponent);
                }
            }
        }

        // Return all active power-ups to the pool
        private static void ReturnActivePowerUps()
        {
            var activePowerUps = GameObject.FindGameObjectsWithTag("PowerUp"); // Find all active power-ups
            foreach (var powerUp in activePowerUps)
            {
                var powerUpComponent = powerUp.GetComponent<PowerUp>(); // Ensure it has the power-up component
                if (powerUpComponent != null)
                {
                    PowerUpPool.Instance.ReturnObject(powerUpComponent);
                }
            }
        }

        // Return all active chest coins to the pool
        private static void ReturnActiveChestCoins()
        {
            var activeChestCoins = GameObject.FindGameObjectsWithTag("ChestCoin");
            foreach (var chestCoin in activeChestCoins)
            {
                var chestCoinsComponent = chestCoin.GetComponent<CoinPickup>();
                if (chestCoinsComponent != null)
                {
                    ChestCoinPool.Instance.ReturnObject(chestCoinsComponent);
                }
            }
        }
        
        // Return all active chests to the pool
        private static void ReturnActiveChests()
        {
            var activeGoldenChests = GameObject.FindGameObjectsWithTag("GoldChest");
            var activeWoodenChests = GameObject.FindGameObjectsWithTag("WoodChest");
            
            foreach (var goldenChests in activeGoldenChests)
            {
                var goldChestComponent = goldenChests.GetComponent<Chest>();
                if (goldChestComponent != null)
                {
                    goldChestComponent.ResetChest();
                    GoldenChestPool.Instance.ReturnObject(goldChestComponent);
                }
            }

            foreach (var woodenChests in activeWoodenChests)
            {
                var woodChestComponent = woodenChests.GetComponent<Chest>();
                if (woodChestComponent != null)
                {
                    woodChestComponent.ResetChest();
                    WoodenChestPool.Instance.ReturnObject(woodChestComponent);
                }
            }
        }
        
        // Return all active projectiles to the pool
        private static void ReturnActiveProjectiles()
        {
            var activePlayerProjectiles = GameObject.FindGameObjectsWithTag("PlayerProjectile");
            var activeSkeletonProjectiles = GameObject.FindGameObjectsWithTag("SkeletonProjectile");

            foreach (var playerProjectile in activePlayerProjectiles)
            {
                var playerProjectileComponent = playerProjectile.GetComponent<Projectile>();
                if (playerProjectileComponent != null)
                {
                    PlayerProjectilePool.Instance.ReturnObject(playerProjectileComponent);
                }
            }

            foreach (var skeletonProjectile in activeSkeletonProjectiles)
            {
                var skeletonProjectileComponent = skeletonProjectile.GetComponent<Projectile>();
                if (skeletonProjectileComponent != null)
                {
                    EnemyProjectilePool.Instance.ReturnObject(skeletonProjectileComponent);
                }
            }
        }
        
        // Return all active explosion to the pool
        private static void ReturnActiveExplosion()
        {
            var activePlayerExplosion = GameObject.FindGameObjectsWithTag("PlayerExplosion");
            var activeSkeletonExplosion = GameObject.FindGameObjectsWithTag("SkeletonExplosion");
            
            foreach (var playerExplosion in activePlayerExplosion)
            {
                var playerExplosionComponent = playerExplosion.GetComponent<Explosion>();
                if (playerExplosionComponent != null)
                {
                    PlayerExplosionPool.Instance.ReturnObject(playerExplosionComponent);
                }
            }

            foreach (var skeletonExplosion in activeSkeletonExplosion)
            {
                var skeletonExplosionComponent = skeletonExplosion.GetComponent<Explosion>();
                if (skeletonExplosionComponent != null)
                {
                    EnemyExplosionPool.Instance.ReturnObject(skeletonExplosionComponent);
                }
            }
        }
        
        // Return all active arrows to the pool
        private static void ReturnActiveArrows()
        {
            var activeArrows = GameObject.FindGameObjectsWithTag("Dart");
            
            foreach (var arrows in activeArrows)
            {
                var arrowComponent = arrows.GetComponent<Arrow>();
                if (arrowComponent != null)
                {
                    ArrowPool.Instance.ReturnObject(arrowComponent);
                }
            }
        }
        #endregion
    }
}

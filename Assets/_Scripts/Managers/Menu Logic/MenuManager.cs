using System;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.Pickups;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers.Menu_Logic
{
    public class MenuManager : MonoBehaviour
    {

        public void StartGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("EmScene");
        }

        public void BackToMenu()
        {
            GameManager.Instance.SaveGameData();
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

        #endregion
    }
}

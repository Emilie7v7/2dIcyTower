using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.Managers.GameOver_Logic
{
    public class GameOverManager : MonoBehaviour
    {
        public static GameOverManager Instance { get; private set; }
        [SerializeField] private GameObject gameOverUI;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void TriggerGameOver()
        {
            StartCoroutine(ShowGameOverWithDelay());
        }

        private IEnumerator ShowGameOverWithDelay()
        {
            yield return new WaitForSeconds(1); // Wait for death animation
            gameOverUI.SetActive(true);
            Time.timeScale = 0;
        }
    }
}

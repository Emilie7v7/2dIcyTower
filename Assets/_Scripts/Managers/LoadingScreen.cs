using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.Managers
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private string gameSceneName = "GameScene";
        [SerializeField] private Image progressBar;
        [SerializeField] private Text loadingText;

        private void Start()
        {
            StartCoroutine(LoadSceneAsync(gameSceneName));
        }
    
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            // Start loading the game scene immediately
            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            // Wait for a frame to ensure loading has properly started
            yield return null;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                
                if (progressBar != null) progressBar.fillAmount = progress;
                if (loadingText != null) loadingText.text = $"Loading... {(progress * 100).ToString("F0")}%";

                if (operation.progress >= 0.9f)
                {
                    // The scene is fully loaded at this point
                    yield return new WaitForSeconds(1f);
                    
                    // Pre-activate any heavy systems here if needed
                    // yield return StartCoroutine(PreloadGameSystems());
                    
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        // Optional: If you have specific systems that need initialization
        private IEnumerator PreloadGameSystems()
        {
            // Initialize any heavy systems here
            // For example, object pools, audio system, etc.
            yield return null;
        }
    }
}
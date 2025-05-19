using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Add this if you have a progress bar

namespace _Scripts.Managers
{
    public class LoadingScreen : MonoBehaviour
    {
        private static LoadingScreen _instance;
        
        [SerializeField] private Canvas loadingCanvas; // Reference to your loading UI canvas
        // Optional: Add references to UI elements
        // [SerializeField] private Text loadingText;
        // [SerializeField] private Image progressBar;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                loadingCanvas.gameObject.SetActive(false); // Hide loading screen at start
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName)
        {
            loadingCanvas.gameObject.SetActive(true); // Show loading screen
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                // Optional: Update UI elements
                // if (progressBar != null) progressBar.fillAmount = progress;
                // if (loadingText != null) loadingText.text = $"Loading... {progress * 100}%";

                if (operation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(1f); // Optional: minimum time to show loading screen
                    loadingCanvas.gameObject.SetActive(false); // Hide loading screen
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
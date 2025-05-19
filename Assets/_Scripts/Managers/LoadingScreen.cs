using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers
{
    public class LoadingScreen : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    
        private static IEnumerator LoadSceneAsync(string sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation != null)
            {
                operation.allowSceneActivation = false;

                while (!operation.isDone)
                {
                    // When the loading is complete
                    if (operation.progress >= 0.9f)
                    {
                        operation.allowSceneActivation = true;
                    }

                    yield return null;
                }
            }
        }
    }
}
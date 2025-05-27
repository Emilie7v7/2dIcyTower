using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }
    
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
        public void LoadScene(string sceneName)
        {
            // Simply load the loading scene
            SceneManager.LoadScene("LoadingScene");
        }
    }
}
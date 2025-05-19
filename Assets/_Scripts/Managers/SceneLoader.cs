using UnityEngine;

namespace _Scripts.Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }
    
        [SerializeField] private LoadingScreen loadingScreen;
    
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
            loadingScreen.LoadScene(sceneName);
        }
    }
}
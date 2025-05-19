using UnityEngine;

namespace _Scripts.Managers
{
    public class InitialLoader : MonoBehaviour
    {
        void Start()
        {
            SceneLoader.Instance.LoadScene("MenuScene");
        }
    }
}


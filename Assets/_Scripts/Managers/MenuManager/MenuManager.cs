using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers.MenuManager
{
    public class MenuManager : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("EmScene");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

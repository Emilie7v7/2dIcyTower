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

        public void BackToMenu()
        {
            GameManager.GameManager.Instance.SaveGameData();
            SceneManager.LoadScene("MenuScene");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

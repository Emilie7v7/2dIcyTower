using _Scripts.Managers.Game_Manager_Logic;
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
            GameManager.Instance.SaveGameData();
            SceneManager.LoadScene("MenuScene");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

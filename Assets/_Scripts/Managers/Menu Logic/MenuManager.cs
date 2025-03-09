using _Scripts.Managers.Game_Manager_Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers.Menu_Logic
{
    public class MenuManager : MonoBehaviour
    {
        public void StartGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("EmScene");
        }

        public void BackToMenu()
        {
            GameManager.Instance.SaveGameData();
            Time.timeScale = 1;
            SceneManager.LoadScene("MenuScene");
        }
        
        public void RestartGame()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }
    }
}

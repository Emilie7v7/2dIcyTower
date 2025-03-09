using _Scripts.Managers.Game_Manager_Logic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers.UI_Logic
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button resetButton;

        private void Start()
        {
            resetButton.onClick.AddListener(ResetGameData);
        }

        private static void ResetGameData()
        {
            var gameManager = FindObjectOfType<GameManager>();

            if (gameManager != null)
            {
                gameManager.ResetGameData();
            }
            else
            {
                Debug.LogError("GameManager Not Found After Scene Reload!");
            }
        }
    }
}

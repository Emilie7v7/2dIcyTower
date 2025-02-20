using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using _Scripts.Managers.GameManager;

namespace _Scripts.Managers.UI
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
            var gameManager = FindObjectOfType<GameManager.GameManager>();

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

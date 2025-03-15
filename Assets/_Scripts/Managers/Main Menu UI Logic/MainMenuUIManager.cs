using System;
using _Scripts.Managers.Save_System_Logic;
using _Scripts.Managers.Score_Logic;
using TMPro;
using UnityEngine;

namespace _Scripts.Managers.Main_Menu_UI_Logic
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text highScoreText;

        private void Start()
        {
            var highScore = SaveSystem.LoadData().highScore;
            highScoreText.text = $"High Score: {highScore}";
        }
    }
}

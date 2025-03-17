using _Scripts.Managers.Save_System_Logic;
using TMPro;
using UnityEngine;

namespace _Scripts.Managers.UI_Logic
{
    public class MainMenuUIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text highScoreText;

        private void Start()
        {
            var highScore = SaveSystem.LoadPlayerData().highScore;
            highScoreText.text = $"Record: {highScore}!";
        }
    }
}

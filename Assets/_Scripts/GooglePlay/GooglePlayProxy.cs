using UnityEngine;

namespace _Scripts.GooglePlay
{
    public class GooglePlayUIProxy : MonoBehaviour
    {
        public void OnSignInButton()
        {
            GooglePlayManager.instance?.SignIn();
        }

        public void OnShowLeaderboardButton()
        {
            GooglePlayManager.instance?.ShowLeaderboardUI();
        }

        public void OnShowAchievementsButton()
        {
            GooglePlayManager.instance?.ShowAchievementsUI();
        }
    }
}

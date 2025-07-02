using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace _Scripts.GooglePlay
{
    public class GooglePlayManager : MonoBehaviour
    {
        public static GooglePlayManager Instance;

        [Header("Leaderboard ID")]
        [SerializeField] private string leaderboardID = "YOUR_LEADERBOARD_ID";

        [Header("Achievement ID (optional)")]
        [SerializeField] private string exampleAchievementID = "YOUR_ACHIEVEMENT_ID";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                InitializePlayGames();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePlayGames()
        {
            PlayGamesPlatform.DebugLogEnabled = true;

            PlayGamesPlatform.Activate();
            Debug.Log("Google Play Games Initialized.");
            
        }

        #region Authentication

        public void SignIn(Action<bool> callback = null)
        {
            if (IsSignedIn())
            {
                Debug.Log("Already signed in.");
                callback?.Invoke(true);
                return;
            }

            Social.localUser.Authenticate(success =>
            {
                if (success)
                {
                    Debug.Log("Signed in successfully.");
                    callback?.Invoke(true);
                }
                else
                {
                    Debug.LogWarning("Sign-in failed or canceled.");
                    callback?.Invoke(false);
                }
            });
        }

        public bool IsSignedIn()
        {
            return Social.localUser.authenticated;
        }

        #endregion

        #region Leaderboards

        public void ShowLeaderboardUI()
        {
            if (IsSignedIn())
            {
                Social.ShowLeaderboardUI();
            }
            else
            {
                Debug.LogWarning("Not signed in. Attempting sign-in...");
                SignIn(success =>
                {
                    if (success)
                        Social.ShowLeaderboardUI();
                });
            }
        }

        public void ReportScore(long score)
        {
            if (IsSignedIn())
            {
                Social.ReportScore(score, leaderboardID, success =>
                {
                    Debug.Log(success ? "Score reported successfully." : "Failed to report score.");
                });
            }
            else
            {
                Debug.LogWarning("Not signed in. Attempting sign-in...");
                SignIn(success =>
                {
                    if (success)
                        ReportScore(score);
                });
            }
        }

        #endregion

        #region Achievements

        public void ShowAchievementsUI()
        {
            if (IsSignedIn())
            {
                Social.ShowAchievementsUI();
            }
            else
            {
                Debug.LogWarning("Not signed in. Attempting sign-in...");
                SignIn(success =>
                {
                    if (success)
                        Social.ShowAchievementsUI();
                });
            }
        }

        public void UnlockAchievement(string achievementID)
        {
            if (IsSignedIn())
            {
                Social.ReportProgress(achievementID, 100.0f, success =>
                {
                    Debug.Log(success ? $"Achievement {achievementID} unlocked." : $"Failed to unlock achievement {achievementID}.");
                });
            }
            else
            {
                Debug.LogWarning("Not signed in. Attempting sign-in...");
                SignIn(success =>
                {
                    if (success)
                        UnlockAchievement(achievementID);
                });
            }
        }

        public void UnlockExampleAchievement()
        {
            UnlockAchievement(exampleAchievementID);
        }

        #endregion
    }
}
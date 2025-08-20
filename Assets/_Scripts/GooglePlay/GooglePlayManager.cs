// GooglePlayManagerV210.cs
using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine.SceneManagement;

namespace _Scripts.GooglePlay
{
    public class GooglePlayManager : MonoBehaviour
    {
        public static GooglePlayManager instance;

        // Keep your IDs
        private readonly string leaderboardID = GPGSIds.leaderboard_spiremageleaderboard;

        [Header("Achievement ID (optional)")] [SerializeField]
        private string exampleAchievementID = "YOUR_ACHIEVEMENT_ID";
        
        public event Action<bool, string> OnAuthStateChanged;

        private bool _initialized;

        private void Awake()
        {
            if (instance != this && instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Coming back to menu or any scene - refresh UI and attempt silent reauth if needed
            if (IsSignedIn())
                RaiseAuthChanged(true);
            else
                SignInSilently();
        }

        private void Start()
        {
            InitializePlayGamesOnce();
            // Auto-auth on start to mirror your original behavior
            SignIn();
        }

        // ---------------- Init ----------------

        private void InitializePlayGamesOnce()
        {
            if (_initialized) return;

            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            Debug.Log($"[GPG] Init - package: {Application.identifier}");

            PlayGamesPlatform.Instance.Authenticate(OnAuth);

            _initialized = true;
        }

        private void OnAuth(SignInStatus status)
        {
            var authed = PlayGamesPlatform.Instance.localUser != null &&
                         PlayGamesPlatform.Instance.localUser.authenticated;
            var code = (int)status;

            Debug.Log($"[GPG] Auth callback. Status: {status} ({code}), Authenticated: {authed}");

            if (authed)
            {
                var p = PlayGamesPlatform.Instance;
                var name = p.GetUserDisplayName();
                var id = p.GetUserId();
                var img = p.GetUserImageUrl();

                Debug.Log($"[GPG] Success. Name: {name}, ID: {id}, Image URL: {img}");
            }
        }
        
        // ---------------- Authentication ----------------

        // Signature kept identical to your original
        public void SignIn(Action<bool> callback = null)
        {
            if (IsSignedIn())
            {
                Debug.Log("[GPG] Already signed in.");
                callback?.Invoke(true);
                return;
            }

            Debug.Log("[GPG] Starting authentication...");
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                bool authed = IsSignedIn();
                Debug.Log($"[GPG] Authenticate result: {status} ({(int)status}) | authenticated={authed}");

                if (authed)
                {
                    var p = PlayGamesPlatform.Instance;
                    Debug.Log(
                        $"[GPG] Player: name='{p.GetUserDisplayName()}', id='{p.GetUserId()}', image='{p.GetUserImageUrl()}'");
                }

                callback?.Invoke(authed);
            });
        }

        // Silent sign in used on Start, scene load, and app resume
        private void SignInSilently()
        {
            InitializePlayGamesOnce();

            if (IsSignedIn())
            {
                RaiseAuthChanged(true);
                return;
            }

            Debug.Log("[GPG] Attempting silent sign in...");
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                var authed = IsSignedIn();
                Debug.Log($"[GPG] Silent auth result: {status} ({(int)status}) | authenticated={authed}");
                RaiseAuthChanged(authed);
                if (!authed) Debug.Log("[GPG] Silent sign in failed - will remain signed out until user taps Sign In.");
            });
        }

        private bool IsSignedIn()
        {
            return Social.localUser != null && Social.localUser.authenticated;
        }
        
        private void RaiseAuthChanged(bool signedIn)
        {
            var nick = signedIn && Social.localUser != null ? Social.localUser.userName : string.Empty;

            OnAuthStateChanged?.Invoke(signedIn, nick);
            Debug.Log($"[GPG] Auth state - {(signedIn ? "signed in" : "signed out")} {nick}");
        }
        
        public void ForceRefreshAuthUI()
        {
            RaiseAuthChanged(IsSignedIn());
        }

        // ---------------- Leaderboards ----------------

        #region Leaderboards

        public void ShowLeaderboardUI()
        {
            Debug.Log("[GPG] Leaderboard button pressed.");
            if (IsSignedIn())
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardID);
            }
            else
            {
                Debug.LogWarning("[GPG] Not signed in. Attempting sign in first...");
                SignIn(success =>
                {
                    if (success) PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardID);
                    else Debug.LogWarning("[GPG] Sign in failed. Leaderboard UI will not be shown.");
                });
            }
        }

        public void ReportScore(long score)
        {
            if (IsSignedIn())
            {
                Debug.Log($"[GPG] Reporting score {score} to '{leaderboardID}'...");
                Social.ReportScore(score, leaderboardID,
                    success =>
                    {
                        Debug.Log(success ? "[GPG] Score reported successfully." : "[GPG] Failed to report score.");
                    });
            }
            else
            {
                Debug.LogWarning("[GPG] Not signed in. Attempting sign in before reporting score...");
                SignIn(success =>
                {
                    if (success) ReportScore(score);
                    else Debug.LogWarning("[GPG] Sign in failed. Score not reported.");
                });
            }
        }

        #endregion

        // ---------------- Achievements ----------------

        #region Achievements

        public void ShowAchievementsUI()
        {
            if (IsSignedIn())
            {
                Debug.Log("[GPG] Showing achievements UI...");
                Social.ShowAchievementsUI();
            }
            else
            {
                Debug.LogWarning("[GPG] Not signed in. Attempting sign in before showing achievements...");
                SignIn(success =>
                {
                    if (success) Social.ShowAchievementsUI();
                    else Debug.LogWarning("[GPG] Sign in failed. Achievements UI will not be shown.");
                });
            }
        }

        public void UnlockAchievement(string achievementID)
        {
            if (IsSignedIn())
            {
                Debug.Log($"[GPG] Unlocking achievement '{achievementID}'...");
                Social.ReportProgress(achievementID, 100.0f,
                    success =>
                    {
                        Debug.Log(success
                            ? $"[GPG] Achievement '{achievementID}' unlocked."
                            : $"[GPG] Failed to unlock achievement '{achievementID}'.");
                    });
            }
            else
            {
                Debug.LogWarning("[GPG] Not signed in. Attempting sign in before unlocking achievement...");
                SignIn(success =>
                {
                    if (success) UnlockAchievement(achievementID);
                    else Debug.LogWarning("[GPG] Sign in failed. Achievement not unlocked.");
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
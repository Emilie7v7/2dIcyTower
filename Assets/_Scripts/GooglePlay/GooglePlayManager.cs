using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace _Scripts.GooglePlay
{
    public class GooglePlayManager : MonoBehaviour
    {
        public static GooglePlayManager instance;

        private readonly string leaderboardID = GPGSIds.leaderboard_spiremageleaderboard;

        [Header("Achievement ID (optional)")]
        [SerializeField] private string exampleAchievementID = "YOUR_ACHIEVEMENT_ID";

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializePlayGames();
            LogEnvironmentBasics();
            GooglePlayDiagnostics.LogPlayServicesAvailability(); // device level check
            SignIn(); // single auth path
        }

        private void InitializePlayGames()
        {
            // Configure as needed. Add .EnableSavedGames() or server auth if you use them.
            var config = new PlayGamesClientConfiguration.Builder()
                //.RequestServerAuthCode(false)
                //.EnableSavedGames()
                .Build();

            PlayGamesPlatform.InitializeInstance(config);

            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            Debug.Log("[GPG] Initialized Play Games Platform.");
        }

        private static void LogEnvironmentBasics()
        {
            Debug.Log($"[GPG] AppId: {Application.identifier} | Version: {Application.version} | Platform: {Application.platform}");
        }

        #region Authentication

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
                Debug.Log($"[GPG] Authenticate result: {status} | Authenticated flag: {authed}");
                Debug.Log($"[GPG] Explanation: {GooglePlayDiagnostics.ExplainSignInStatus(status)}");

                if (status == SignInStatus.Success)
                {
                    var p = PlayGamesPlatform.Instance;
                    Debug.Log($"[GPG] Player: name='{p.GetUserDisplayName()}', id='{p.GetUserId()}', image='{p.GetUserImageUrl()}'");
                    callback?.Invoke(true);
                }
                else
                {
                    // Extra device level signal on failure
                    GooglePlayDiagnostics.LogPlayServicesAvailability();
                    callback?.Invoke(false);
                }
            });
        }

        public bool IsSignedIn()
        {
            return Social.localUser != null && Social.localUser.authenticated;
        }

        public void ManuallySignIn(Action<bool> callback = null)
        {
            Debug.Log("[GPG] Manual auth requested...");
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.ManuallyAuthenticate(status =>
            {
                bool authed = IsSignedIn();
                Debug.Log($"[GPG] Manual Authenticate result: {status} | Authenticated flag: {authed}");
                Debug.Log($"[GPG] Explanation: {GooglePlayDiagnostics.ExplainSignInStatus(status)}");
                if (status != SignInStatus.Success) GooglePlayDiagnostics.LogPlayServicesAvailability();
                callback?.Invoke(status == SignInStatus.Success);
            });
#else
            Debug.LogWarning("[GPG] ManualAuthenticate is Android only.");
            callback?.Invoke(false);
#endif
        }

        #endregion

        #region Leaderboards

        public void ShowLeaderboardUI()
        {
            Debug.Log("[GPG] Leaderboard button pressed.");
            if (IsSignedIn())
            {
                Debug.Log("[GPG] Showing leaderboard UI for configured ID...");
                PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardID);
            }
            else
            {
                Debug.LogWarning("[GPG] Not signed in. Attempting sign in before showing leaderboard...");
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
                Social.ReportScore(score, leaderboardID, success =>
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
                Social.ReportProgress(achievementID, 100.0f, success =>
                {
                    Debug.Log(success ? $"[GPG] Achievement '{achievementID}' unlocked." : $"[GPG] Failed to unlock achievement '{achievementID}'.");
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

    /// <summary>
    /// Centralized diagnostics for Google Play Games. Explains statuses and checks Play Services.
    /// </summary>
    internal static class GooglePlayDiagnostics
    {
        public static string ExplainSignInStatus(SignInStatus status)
        {
            switch (status)
            {
                case SignInStatus.Success:
                    return "Success. The user is authenticated.";

                case SignInStatus.UiSignInRequired:
                    return "UI sign in required. You must call ManuallyAuthenticate or trigger a UI flow. Often occurs when silent sign in is not possible.";

                case SignInStatus.Canceled:
                    return "User canceled the sign in UI. The user backed out or closed the dialog.";

                case SignInStatus.NetworkError:
                    return "Network error. No connectivity or flaky connection. Check internet, captive portals, or airplane mode.";

                case SignInStatus.Timeout:
                    return "Timed out waiting for Google services. Device may have poor connectivity or Play Services is updating.";

                case SignInStatus.DeveloperError:
                    return "Developer error. Common causes: package name or app signing certificate mismatch, wrong OAuth client in Play Console, or not using the correct applicationId.";

                case SignInStatus.AlreadyInProgress:
                    return "A sign in attempt is already in progress. Debounce your calls or wait for the current auth to complete.";

                case SignInStatus.BadAuthentication:
                    return "Bad authentication. Account token invalid or revoked. Try removing the account from device, clear Google Play Games data, and sign in again.";

                case SignInStatus.NotAuthorized:
                    return "Not authorized. The user disabled Google Play Games access for your app or did not grant required scopes.";

                case SignInStatus.InternalError:
                    return "Internal error from Google Play Games services. Check logcat for GooglePlayGames and GamesSignIn tags. Often transient.";

                default:
                    return "Unknown status. Check logcat for GooglePlayGames and GoogleSignIn tags for more detail.";
            }
        }

        /// <summary>
        /// Checks if Google Play Services is available and logs a readable reason if not.
        /// </summary>
        public static void LogPlayServicesAvailability()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var gaa = new AndroidJavaClass("com.google.android.gms.common.GoogleApiAvailability"))
                {
                    var instance = gaa.CallStatic<AndroidJavaObject>("getInstance");
                    int code = instance.Call<int>("isGooglePlayServicesAvailable", activity);
                    string txt = ExplainConnectionResult(code);
                    Debug.Log($"[GPG] Google Play Services availability code: {code} -> {txt}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GPG] Could not query Google Play Services availability: {e.Message}");
            }
#else
            Debug.Log("[GPG] Play Services availability check skipped. Not running on Android device.");
#endif
        }

        private static string ExplainConnectionResult(int code)
        {
            // Based on com.google.android.gms.common.ConnectionResult constants
            switch (code)
            {
                case 0: return "SUCCESS. Play Services OK.";
                case 1: return "SERVICE_MISSING. Play Services not installed.";
                case 2: return "SERVICE_VERSION_UPDATE_REQUIRED. Update Play Services in Play Store.";
                case 3: return "SERVICE_DISABLED. Play Services disabled on device.";
                case 4: return "SIGN_IN_REQUIRED. User must sign in to Google on device.";
                case 5: return "INVALID_ACCOUNT. The provided account is invalid on this device.";
                case 6: return "RESOLUTION_REQUIRED. User action required to fix Play Services.";
                case 7: return "NETWORK_ERROR. No or unstable network.";
                case 8: return "INTERNAL_ERROR. Google services internal failure.";
                case 9: return "SERVICE_INVALID. The installed Play Services is invalid.";
                case 10: return "DEVELOPER_ERROR. Likely config mismatch in app id, package, or SHA certificate.";
                case 11: return "LICENSE_CHECK_FAILED. Licensing failure.";
                case 13: return "CANCELED. Operation canceled.";
                case 14: return "TIMEOUT. Operation timed out.";
                case 15: return "INTERRUPTED. Operation interrupted.";
                case 16: return "API_UNAVAILABLE. Requested API unavailable on this device.";
                case 17: return "SIGN_IN_FAILED. Generic sign in failure.";
                case 18: return "SERVICE_UPDATING. Play Services currently updating.";
                case 19: return "SERVICE_MISSING_PERMISSION. Missing required permission.";
                case 20: return "RESTRICTED_PROFILE. Restricted profile prevents sign in.";
                case 21: return "API_DISABLED. API disabled on device.";
                case 22: return "API_DISABLED_FOR_CONNECTION. API disabled for this connection.";
                default: return "Unknown Play Services error code.";
            }
        }
    }
}

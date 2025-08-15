// GooglePlayManagerV210.cs
using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;

namespace _Scripts.GooglePlay
{
    public class GooglePlayManager : MonoBehaviour
    {
        public static GooglePlayManager instance;

        // Keep your IDs
        private readonly string leaderboardID = GPGSIds.leaderboard_spiremageleaderboard;

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI alreadySignInText;

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
            // Auto-auth on start to mirror your original behavior
            SignIn();
        }

        private void InitializePlayGames()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            Debug.Log($"[GPG] Starting auto-auth. Package: {Application.identifier}");
            GooglePlayDiagnosticsV210.LogPlayServicesAvailability();

            PlayGamesPlatform.Instance.Authenticate(OnAuth);
        }
        
        private void OnAuth(SignInStatus status)
        {
            var authed = PlayGamesPlatform.Instance.localUser != null && PlayGamesPlatform.Instance.localUser.authenticated;
            var code = (int)status;
            var hint = GooglePlayDiagnosticsV210.Explain(status);

            Debug.Log($"[GPG] Auth callback. Status: {status} ({code}), Authenticated: {authed}");
            Debug.Log($"[GPG] Hint: {hint}");

            if (authed)
            {
                var p = PlayGamesPlatform.Instance;
                var name = p.GetUserDisplayName();
                var id = p.GetUserId();
                var img = p.GetUserImageUrl();

                Debug.Log($"[GPG] Success. Name: {name}, ID: {id}, Image URL: {img}");
                if (text) text.text = $"Signed in as:\n{name}";
            }
            else
            {
                if (text) text.text = $"Sign in failed: {status}\n{hint}\nTap Retry.";
                GooglePlayDiagnosticsV210.LogAllPrechecks();
            }
        }
        private static void LogEnvironmentBasics()
        {
            Debug.Log($"[GPG] AppId: {Application.identifier} | Version: {Application.version} | Platform: {Application.platform}");
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log($"[GPG] Device: {SystemInfo.deviceModel} | OS: {SystemInfo.operatingSystem}");
#endif
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
                Debug.Log($"[GPG] Hint: {GooglePlayDiagnosticsV210.Explain(status)}");

                if (authed)
                {
                    var p = PlayGamesPlatform.Instance;
                    Debug.Log($"[GPG] Player: name='{p.GetUserDisplayName()}', id='{p.GetUserId()}', image='{p.GetUserImageUrl()}'");
                }
                else
                {
                    GooglePlayDiagnosticsV210.LogAllPrechecks();
                }

                callback?.Invoke(authed);
            });
        }

        private bool IsSignedIn()
        {
            return Social.localUser != null && Social.localUser.authenticated;
        }

        // Optional helper if you want a manual button. Not used automatically.
        public void ManuallySignIn()
        {
            if (IsSignedIn())
            {
                alreadySignInText.text = "Already signed in.";
            }
            else
            {
                Debug.Log("[GPG] Manual auth requested.");
                PlayGamesPlatform.Instance.ManuallyAuthenticate(status =>
                {
                    bool authed = IsSignedIn();
                    Debug.Log($"[GPG] Manual auth result: {status} ({(int)status}) | authenticated={authed}");
                    Debug.Log($"[GPG] Hint: {GooglePlayDiagnosticsV210.Explain(status)}");
                    if (!authed) GooglePlayDiagnosticsV210.LogAllPrechecks();
                });
                Debug.LogWarning("[GPG] ManualAuthenticate is Android only.");
            }
        }

        // ---------------- Leaderboards ----------------

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

        // ---------------- Achievements ----------------

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
    }

    // ---------- Diagnostics helper tailored for v2.1.0 ----------
    internal static class GooglePlayDiagnosticsV210
    {
        public static string Explain(GooglePlayGames.BasicApi.SignInStatus s)
        {
            switch (s)
            {
                case SignInStatus.Success:
                    return "Success - user authenticated.";
                case SignInStatus.Canceled:
                    return "User canceled the sign in UI.";
                case SignInStatus.InternalError:
                    return "Internal error - check logcat for GamesSignIn or GoogleSignIn.";
                default:
                    return "Unknown status - check logcat.";
            }
        }

        public static void LogAllPrechecks()
        {
            LogPlayServicesAvailability();
            LogPlayGamesAppPresence();
            LogHasGoogleAccount();
            LogNetworkState();
            LogSigningCertSha1();
        }

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
                    Debug.Log($"[GPG] Play Services availability: {code} - {ExplainConn(code)}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GPG] Play Services check failed: {e.Message}");
            }
#else
            Debug.Log("[GPG] Play Services check skipped - not on Android device.");
#endif
        }

        public static void LogPlayGamesAppPresence()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var pm = activity.Call<AndroidJavaObject>("getPackageManager"))
                {
                    const string pkg = "com.google.android.play.games";
                    var appInfo = pm.Call<AndroidJavaObject>("getApplicationInfo", pkg, 0);
                    bool enabled = appInfo.Get<bool>("enabled");
                    Debug.Log($"[GPG] Play Games app installed: yes - enabled={enabled}");
                }
            }
            catch
            {
                Debug.LogWarning("[GPG] Play Games app installed: no");
            }
#else
            Debug.Log("[GPG] Play Games app check skipped - not on Android device.");
#endif
        }

        public static void LogHasGoogleAccount()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var accountManagerClass = new AndroidJavaClass("android.accounts.AccountManager"))
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    var am = accountManagerClass.CallStatic<AndroidJavaObject>("get", activity);
                    var accounts = am.Call<AndroidJavaObject[]>("getAccountsByType", "com.google");
                    int count = accounts != null ? accounts.Length : 0;
                    Debug.Log($"[GPG] Google accounts on device: {count}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GPG] Could not query Google accounts: {e.Message}");
            }
#else
            Debug.Log("[GPG] Account check skipped - not on Android device.");
#endif
        }

        public static void LogNetworkState()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var cm = activity.Call<AndroidJavaObject>("getSystemService", "connectivity"))
                {
                    var active = cm.Call<AndroidJavaObject>("getActiveNetworkInfo");
                    bool connected = active != null && active.Call<bool>("isConnected");
                    string type = active != null ? active.Call<int>("getType").ToString() : "none";
                    Debug.Log($"[GPG] Network connected={connected} type={type}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GPG] Network state check failed: {e.Message}");
            }
#else
            Debug.Log("[GPG] Network check skipped - not on Android device.");
#endif
        }

        public static void LogSigningCertSha1()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var pm = activity.Call<AndroidJavaObject>("getPackageManager"))
                using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                {
                    int sdk = version.GetStatic<int>("SDK_INT");
                    string pkg = activity.Call<string>("getPackageName");
                    int flag = sdk >= 28 ? 134217728 : 64; // GET_SIGNING_CERTIFICATES or GET_SIGNATURES

                    var pi = pm.Call<AndroidJavaObject>("getPackageInfo", pkg, flag);
                    string sha1 = TryComputeSha1(pi, sdk);
                    if (!string.IsNullOrEmpty(sha1))
                        Debug.Log($"[GPG] App signing SHA-1: {sha1} - compare with Play Console OAuth client.");
                    else
                        Debug.LogWarning("[GPG] Could not compute signing SHA-1.");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[GPG] SHA-1 read failed: {e.Message}");
            }
#else
            Debug.Log("[GPG] SHA-1 check skipped - not on Android device.");
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static string TryComputeSha1(AndroidJavaObject packageInfo, int sdk)
        {
            // API 28+: signingInfo.apkContentsSigners[0]
            if (sdk >= 28)
            {
                try
                {
                    using (var signingInfo = packageInfo.Get<AndroidJavaObject>("signingInfo"))
                    {
                        if (signingInfo != null)
                        {
                            var signers = signingInfo.Call<AndroidJavaObject[]>("getApkContentsSigners");
                            if (signers != null && signers.Length > 0)
                                return DigestToHexSha1(signers[0]);
                        }
                    }
                }
                catch { /* fall back */ }
            }

            // Legacy: signatures[0]
            try
            {
                var sigs = packageInfo.Get<AndroidJavaObject[]>("signatures");
                if (sigs != null && sigs.Length > 0)
                    return DigestToHexSha1(sigs[0]);
            }
            catch { }

            return null;
        }

        private static string DigestToHexSha1(AndroidJavaObject signature)
        {
            // signature.toByteArray returns sbyte[] from Java. Convert to byte[] first.
            sbyte[] sbytes = signature.Call<sbyte[]>("toByteArray");
            byte[] bytes = SBytesToBytes(sbytes);

            using (var mdClass = new AndroidJavaClass("java.security.MessageDigest"))
            using (var md = mdClass.CallStatic<AndroidJavaObject>("getInstance", "SHA1"))
            {
                md.Call("update", bytes);
                byte[] digest = md.Call<byte[]>("digest");
                return BytesToHex(digest);
            }
        }

        private static byte[] SBytesToBytes(sbyte[] s)
        {
            if (s == null) return null;
            var b = new byte[s.Length];
            for (int i = 0; i < s.Length; i++) b[i] = unchecked((byte)s[i]);
            return b;
        }

        private static string BytesToHex(byte[] bytes)
        {
            if (bytes == null) return null;
            char[] c = new char[bytes.Length * 3 - 1];
            int i = 0;
            for (int b = 0; b < bytes.Length; b++)
            {
                string h = bytes[b].ToString("X2");
                c[i++] = h[0];
                c[i++] = h[1];
                if (b < bytes.Length - 1) c[i++] = ':';
            }
            return new string(c);
        }
#endif

        private static string ExplainConn(int code)
        {
            switch (code)
            {
                case 0: return "SUCCESS";
                case 1: return "SERVICE_MISSING";
                case 2: return "SERVICE_VERSION_UPDATE_REQUIRED";
                case 3: return "SERVICE_DISABLED";
                case 4: return "SIGN_IN_REQUIRED";
                case 5: return "INVALID_ACCOUNT";
                case 7: return "NETWORK_ERROR";
                case 8: return "INTERNAL_ERROR";
                case 9: return "SERVICE_INVALID";
                case 10: return "DEVELOPER_ERROR";
                case 13: return "CANCELED";
                case 14: return "TIMEOUT";
                case 16: return "API_UNAVAILABLE";
                case 17: return "SIGN_IN_FAILED";
                case 18: return "SERVICE_UPDATING";
                case 20: return "RESTRICTED_PROFILE";
                default: return "UNKNOWN";
            }
        }
    }
}
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;

namespace _Scripts.GooglePlay
{
    public class TestGooglePlayV2 : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        void Start()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            Debug.Log($"[GPG] Starting auto-auth. Package: {Application.identifier}");
            GooglePlayDiagnostics.LogPlayServicesAvailability();

            PlayGamesPlatform.Instance.Authenticate(OnAuth);
        }

        public void RetrySignIn()
        {
            Debug.Log("[GPG] Manual auth triggered");
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.ManuallyAuthenticate(OnAuth);
#else
        Debug.LogWarning("[GPG] ManualAuthenticate is Android only.");
#endif
        }

        private void OnAuth(SignInStatus status)
        {
            bool authed = PlayGamesPlatform.Instance.localUser.authenticated;
            string explain = GooglePlayDiagnostics.ExplainSignInStatus(status);

            Debug.Log($"[GPG] Auth callback. Status: {status}, Authenticated: {authed}");
            Debug.Log($"[GPG] Explanation: {explain}");

            if (status == SignInStatus.Success)
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
                if (text) text.text = $"Sign in failed: {status}\n{explain}\nTap Retry.";
                GooglePlayDiagnostics.LogPlayServicesAvailability();
            }
        }
    }
}

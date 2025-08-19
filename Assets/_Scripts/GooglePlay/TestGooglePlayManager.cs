// TestGooglePlayV2.cs

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
            PlayGamesPlatform.Activate();

            Debug.Log($"[GPG] Starting auto-auth. Package: {Application.identifier}");

            PlayGamesPlatform.Instance.Authenticate(OnAuth);
        }

        public void RetrySignIn()
        {
#if UNITY_ANDROID
            Debug.Log("[GPG] Manual auth triggered");
            PlayGamesPlatform.Instance.ManuallyAuthenticate(OnAuth);
#else
        Debug.LogWarning("[GPG] ManualAuthenticate is Android only.");
#endif
        }

        private void OnAuth(SignInStatus status)
        {
            bool authed = PlayGamesPlatform.Instance.localUser != null && PlayGamesPlatform.Instance.localUser.authenticated;
            int code = (int)status;

            Debug.Log($"[GPG] Auth callback. Status: {status} ({code}), Authenticated: {authed}");

            if (authed)
            {
                var p = PlayGamesPlatform.Instance;
                var name = p.GetUserDisplayName();
                var id = p.GetUserId();
                var img = p.GetUserImageUrl();

                Debug.Log($"[GPG] Success. Name: {name}, ID: {id}, Image URL: {img}");
                if (text) text.text = $"Signed in as:\n{name}";
            }
        }
    }
}
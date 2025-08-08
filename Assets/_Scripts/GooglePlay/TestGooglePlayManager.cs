using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi; // for SignInStatus
using TMPro;

public class TestGooglePlayV2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        // Auto sign-in attempt
        PlayGamesPlatform.Instance.Authenticate(OnAuth);
    }

    // Hook this to a UI button
    public void RetrySignIn()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(OnAuth);
    }

    private void OnAuth(SignInStatus status)
    {
        if (text == null) Debug.LogWarning("TMP text not assigned.");

        if (status == SignInStatus.Success)
        {
            var p = PlayGamesPlatform.Instance;
            var name = p.GetUserDisplayName();
            if (text) text.text = $"Signed in as:\n{name}";
            Debug.Log("GPG sign-in success");
        }
        else
        {
            if (text) text.text = $"Sign in failed: {status}\nTap Retry.";
            Debug.LogWarning($"GPG sign-in failed: {status}");
        }
    }
}

using System;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;

public class TestGooglePlayManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private TextMeshProUGUI text;
    void Start()
    {
        SignIn();
    }

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();
            string imgUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
            
            text.text = "Signed in as: \n" + name;
        }
        else
        {
            text.text = "Sign in failed";
        }
    }
}

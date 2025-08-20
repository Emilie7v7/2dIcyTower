using System;
using UnityEngine;
using TMPro;
using Object = System.Object;

namespace _Scripts.GooglePlay
{
    public class GooglePlayUIBinder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI alreadySignedInText;
        
        private GooglePlayManager googlePlayManager;

        private void OnEnable()
        {
            googlePlayManager = GooglePlayManager.instance ?? FindFirstObjectByType<GooglePlayManager>();
            if (googlePlayManager != null)
            {
                googlePlayManager.OnAuthStateChanged += UpdateUI;
                googlePlayManager.ForceRefreshAuthUI();
            }
        }

        private void OnDisable()
        {
            if (googlePlayManager != null)
            {
                googlePlayManager.OnAuthStateChanged -= UpdateUI;
            }
        }

        private void UpdateUI(bool signedIn, string nick)
        {
            if (statusText)
                statusText.text = signedIn ? $"Signed in as:\n{nick}" : "Not signed in";

            if (alreadySignedInText)
                alreadySignedInText.text = signedIn ? "Already signed in." : string.Empty;
        }
    }
}

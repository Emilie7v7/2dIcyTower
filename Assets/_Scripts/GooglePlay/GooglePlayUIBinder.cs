using System;
using UnityEngine;
using TMPro;
using Object = System.Object;

namespace _Scripts.GooglePlay
{
    public class GooglePlayUIBinder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statusText;
        
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
                statusText.text = signedIn ? $"{nick}" : "Not signed in";
        }
    }
}

using GoogleMobileAds.Ump.Api;
using UnityEngine;

namespace _Scripts.PrivacyPolicy
{
    public class PrivacyPolicy : MonoBehaviour
    {
        // URL to your hosted privacy policy
        private readonly string privacyPolicyURL = "https://sites.google.com/view/aetherbitestudio-privacy/privacy-policy";

        // Call this from a UI Button
        public void OpenPrivacyPolicyLink()
        {
            Application.OpenURL(privacyPolicyURL);
        }

        #region Privacy Policy Settings

        public void ShowPrivacyOptions()
        {
            ConsentForm.Load((ConsentForm form, FormError error) =>
            {
                if (error != null)
                {
                    Debug.LogWarning("Failed to load consent form: " + error.Message);
                    return;
                }

                if (form != null)
                {
                    form.Show((FormError showError) =>
                    {
                        if (showError != null)
                        {
                            Debug.LogWarning("Error showing consent form: " + showError.Message);
                        }
                    });
                }
            });
        }

        #endregion
    
    }
}

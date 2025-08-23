using UnityEngine;
using GoogleMobileAds.Ump.Api;

public class PrivacySettings : MonoBehaviour
{
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
}

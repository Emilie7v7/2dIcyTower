using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

namespace _Scripts.GooglePlay
{
    public class AdConsentManager : MonoBehaviour
    {
        private void Start()
        {
            RequestConsentInfo();
        }

        private static void RequestConsentInfo()
        {
            var parameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };

            ConsentInformation.Update(parameters, (FormError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Consent info update failed: " + error.Message);
                    InitializeAds();
                    return;
                }

                Debug.Log("Consent info updated. Current status: " + ConsentInformation.ConsentStatus);

                if (ConsentInformation.IsConsentFormAvailable() &&
                    ConsentInformation.ConsentStatus == ConsentStatus.Required)
                {
                    LoadAndShowConsentForm();
                }
                else
                {
                    Debug.Log("Consent form not needed. Proceeding to initialize ads.");
                    InitializeAds();
                }
            });
            
            Debug.Log("Consent Status: " + ConsentInformation.ConsentStatus);
            Debug.Log("Is Consent Form Available: " + ConsentInformation.IsConsentFormAvailable());
        }

        private static void LoadAndShowConsentForm()
        {
            ConsentForm.Load((ConsentForm form, FormError loadError) =>
            {
                if (loadError != null)
                {
                    Debug.LogError("Consent form load failed: " + loadError.Message);
                    InitializeAds();
                    return;
                }

                Debug.Log("Consent form loaded successfully.");

                form.Show((FormError showError) =>
                {
                    if (showError != null)
                    {
                        Debug.LogError("Consent form show error: " + showError.Message);
                    }

                    Debug.Log("Consent form closed. Current status: " + ConsentInformation.ConsentStatus);

                    InitializeAds();
                });
            });
        }

        private static void InitializeAds()
        {
            var requestConfiguration = new RequestConfiguration()
            {
                TagForChildDirectedTreatment = TagForChildDirectedTreatment.False,
                TagForUnderAgeOfConsent = TagForUnderAgeOfConsent.False
            };

            MobileAds.SetRequestConfiguration(requestConfiguration);

            MobileAds.Initialize(initStatus =>
            {
                Debug.Log("AdMob initialized.");
            });
        }

        public void ResetConsent()
        {
            ConsentInformation.Reset();
            Debug.Log("Consent reset. Will show form on next launch.");
        }
    }
}
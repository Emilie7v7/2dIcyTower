using UnityEngine;

public class PrivacyPolicyLink : MonoBehaviour
{
    // URL to your hosted privacy policy
    private string privacyPolicyURL = "https://sites.google.com/view/aetherbitestudio-privacy/privacy-policy";

    // Call this from a UI Button
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL(privacyPolicyURL);
    }
}

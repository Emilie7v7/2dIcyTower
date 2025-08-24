using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
#endif

namespace _Scripts.GooglePlay
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance { get; private set; }

        [Header("Use test IDs until release")]
        [SerializeField] private bool useGoogleTestIds = true;

        [Header("Android Ad Unit IDs (set when releasing)")]
        [SerializeField] private string bannerAdUnitId     = "ca-app-pub-3940256099942544/6300978111"; // Test
        [SerializeField] private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // Test
        [SerializeField] private string rewardedAdUnitId     = "ca-app-pub-3940256099942544/5224354917"; // Test

#if UNITY_ANDROID
        private BannerView banner;
        private InterstitialAd interstitial;
        private RewardedAd rewarded;
#endif

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
#if UNITY_ANDROID
            // Safe to keep test device on even with real Ad Unit IDs
            var cfg = new RequestConfiguration();
            cfg.TestDeviceIds = new List<string> { "5F9B68244801867545DE04BF10E6B02E" }; // your Logcat ID
            MobileAds.SetRequestConfiguration(cfg);

            // Run UMP first so we are compliant in the EEA
            RequestConsentThenInitAds();
#else
            // Other platforms: no-op
#endif
        }

#if UNITY_ANDROID
        private void RequestConsentThenInitAds()
        {
            var req = new ConsentRequestParameters();
            ConsentInformation.Update(req, (FormError updateError) =>
            {
                if (updateError != null)
                {
                    Debug.LogWarning("UMP Update error: " + updateError.Message);
                    // Continue anyway. Non-personalized ads will still work.
                    InitializeMobileAds();
                    return;
                }

                Debug.Log("UMP ConsentStatus: " + ConsentInformation.ConsentStatus
                          + " | CanRequestAds: " + ConsentInformation.CanRequestAds()
                          + " | IsFormAvailable: " + ConsentInformation.IsConsentFormAvailable());

                // If a form is available and consent is required, show it.
                ConsentForm.Load((ConsentForm form, FormError loadError) =>
                {
                    if (loadError != null)
                    {
                        Debug.LogWarning("UMP Load error: " + loadError.Message);
                        InitializeMobileAds();
                        return;
                    }

                    if (ConsentInformation.ConsentStatus == ConsentStatus.Required && form != null)
                    {
                        form.Show((FormError showError) =>
                        {
                            if (showError != null)
                                Debug.LogWarning("UMP Show error: " + showError.Message);

                            InitializeMobileAds();
                        });
                    }
                    else
                    {
                        InitializeMobileAds();
                    }
                });
            });
        }

        private void InitializeMobileAds()
        {
            MobileAds.Initialize(_ =>
            {
                Debug.Log("Mobile Ads initialized");
                // Preload full screens
                LoadInterstitial();
                LoadRewarded();
                // Optional: auto show a banner on main menu load
                // ShowBannerBottom();
            });
        }

        private AdRequest BuildRequest()
        {
            // If you need non-personalized only, uncomment below
            // var r = new AdRequest();
            // r.Extras.Add("npa", "1");
            // return r;
            return new AdRequest();
        }

        // ---------- Banner ----------
        public void ShowBannerBottom()
        {
            if (banner != null) return;

            banner = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);

            banner.OnBannerAdLoaded += () => Debug.Log("Banner loaded");
            banner.OnBannerAdLoadFailed += (err) => Debug.LogWarning("Banner load failed: " + err);
            banner.OnAdImpressionRecorded += () => Debug.Log("Banner impression");

            banner.LoadAd(BuildRequest());
        }

        public void HideBanner()
        {
            banner?.Destroy();
            banner = null;
        }

        // ---------- Interstitial ----------
        public void LoadInterstitial()
        {
            InterstitialAd.Load(interstitialAdUnitId, BuildRequest(), (ad, error) =>
            {
                if (error != null) { Debug.LogWarning("Interstitial load failed: " + error); return; }
                interstitial = ad;
                interstitial.OnAdFullScreenContentClosed += () => { interstitial = null; LoadInterstitial(); };
                Debug.Log("Interstitial loaded");
            });
        }

        public bool ShowInterstitial()
        {
            if (interstitial != null && interstitial.CanShowAd())
            {
                interstitial.Show();
                return true;
            }
            Debug.Log("Interstitial not ready");
            return false;
        }

        // ---------- Rewarded ----------
        public void LoadRewarded()
        {
            RewardedAd.Load(rewardedAdUnitId, BuildRequest(), (ad, error) =>
            {
                if (error != null) { Debug.LogWarning("Rewarded load failed: " + error); return; }
                rewarded = ad;
                rewarded.OnAdFullScreenContentClosed += () => { rewarded = null; LoadRewarded(); };
                Debug.Log("Rewarded loaded");
            });
        }

        public bool ShowRewarded(Action<Reward> onReward)
        {
            if (rewarded != null && rewarded.CanShowAd())
            {
                rewarded.Show(r =>
                {
                    try { onReward?.Invoke(r); } catch (Exception e) { Debug.LogException(e); }
                });
                return true;
            }
            Debug.Log("Rewarded not ready");
            return false;
        }
#endif

        private void OnDestroy()
        {
#if UNITY_ANDROID
            banner?.Destroy();
            interstitial?.Destroy();
            rewarded?.Destroy();
#endif
            if (Instance == this) Instance = null;
        }
    }
}

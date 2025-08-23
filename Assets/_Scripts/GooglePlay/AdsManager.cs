using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace _Scripts.GooglePlay
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance { get; private set; }

        [Header("Use test IDs until release")]
        [SerializeField] private bool useGoogleTestIds = true;

        [Header("Android Ad Unit IDs (set when releasing)")]
        [SerializeField] private string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";        // Test
        [SerializeField] private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";  // Test
        [SerializeField] private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";     // Test

        private BannerView banner;
        private InterstitialAd interstitial;
        private RewardedAd rewarded;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID
            if (!useGoogleTestIds)
            {
                // TODO: set your real IDs in the Inspector before release
                // bannerAdUnitId = "YOUR_BANNER_ID";
                // interstitialAdUnitId = "YOUR_INTERSTITIAL_ID";
                // rewardedAdUnitId = "YOUR_REWARDED_ID";
            }
#else
        // Prevent accidental use on other platforms during dev
        useGoogleTestIds = true;
#endif
        }

        private void Start()
        {
            // AdMob App ID is set in Unity: Assets > Google Mobile Ads > Settings
            MobileAds.Initialize(initStatus =>
            {
                Debug.Log("Mobile Ads initialized");
                // Optional: preload an interstitial and rewarded
                LoadInterstitial();
                LoadRewarded();
            });
        }

        private AdRequest BuildRequest()
        {
            // If you ever need to force non-personalized ads manually:
            // var request = new AdRequest();
            // request.Extras.Add("npa", "1");
            // return request;
            return new AdRequest();
        }

        // ===================== Banner =====================
        public void ShowBannerBottom()
        {
            if (banner != null) return;

            banner = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
            banner.LoadAd(BuildRequest());
        }

        public void HideBanner()
        {
            banner?.Destroy();
            banner = null;
        }

        // ===================== Interstitial =====================
        public void LoadInterstitial()
        {
            InterstitialAd.Load(interstitialAdUnitId, BuildRequest(), (ad, error) =>
            {
                if (error != null)
                {
                    Debug.LogWarning("Interstitial load failed: " + error);
                    return;
                }
                interstitial = ad;
                interstitial.OnAdFullScreenContentClosed += () =>
                {
                    interstitial = null;
                    LoadInterstitial(); // prepare next one
                };
            });
        }

        public bool ShowInterstitial()
        {
            if (interstitial != null && interstitial.CanShowAd())
            {
                interstitial.Show();
                return true;
            }
            return false;
        }

        // ===================== Rewarded =====================
        public void LoadRewarded()
        {
            RewardedAd.Load(rewardedAdUnitId, BuildRequest(), (ad, error) =>
            {
                if (error != null)
                {
                    Debug.LogWarning("Rewarded load failed: " + error);
                    return;
                }
                rewarded = ad;
                rewarded.OnAdFullScreenContentClosed += () =>
                {
                    rewarded = null;
                    LoadRewarded(); // prepare next one
                };
            });
        }

        // Returns true if an ad was shown. onReward will be called if user earns reward.
        public bool ShowRewarded(Action<Reward> onReward)
        {
            if (rewarded != null && rewarded.CanShowAd())
            {
                rewarded.Show(reward =>
                {
                    try { onReward?.Invoke(reward); }
                    catch (Exception e) { Debug.LogException(e); }
                });
                return true;
            }
            return false;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            banner?.Destroy();
            interstitial?.Destroy();
            rewarded?.Destroy();
        }
    }
}

using System;
using System.Collections;
using _Scripts.CoreSystem;
using _Scripts.GooglePlay;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.Managers.GameOver_Logic
{
    public class GameOverManager : MonoBehaviour
    {
        public static GameOverManager Instance { get; private set; }
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private Button reviveButton;
        [SerializeField] private PlayerComponent.Player player;
        [SerializeField] private Stats playerStats;
        [SerializeField] private GameObject playerGo;
        
        private AdsManager adsManager;
        private bool _reviveUsed = false;
        private bool lastDeathWasLava = false;
        private Vector3 _revivePosition;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (gameOverUI == null) // Ensure UI reference is valid
            {
                gameOverUI = GameObject.Find("GameOverCanvas"); // Adjust name if necessary
            }

            if (reviveButton != null)
            {
                reviveButton.onClick.AddListener(OnReviveButtonClicked);
            }
            
            if (adsManager == null)
            {
                var adsObj = GameObject.Find("AdsManager");
                if (adsObj != null)
                    adsManager = adsObj.GetComponent<AdsManager>();
            }
        }

        public void TriggerGameOver()
        {
            if (playerGo != null)
                _revivePosition = playerGo.transform.position;
            lastDeathWasLava = player != null && player.DiedByLava;
            StartCoroutine(ShowGameOverWithDelay());
        }

        private IEnumerator ShowGameOverWithDelay()
        {
            yield return new WaitForSeconds(1); // Wait for death animation
            gameOverUI.SetActive(true);
            Time.timeScale = 0;

            if (reviveButton != null)
            {
                reviveButton.gameObject.SetActive(!_reviveUsed || lastDeathWasLava);
            }
        }

        private void OnReviveButtonClicked()
        {
            if (_reviveUsed || lastDeathWasLava) return;

            var shown = adsManager.ShowRewarded(OnAdRewarded);

            if (!shown)
            {
                Debug.Log("Reward ad not ready");
            }
        }

        private void OnAdRewarded(GoogleMobileAds.Api.Reward reward)
        {
            gameOverUI.SetActive(false);
            
            Time.timeScale = 1;

            if (playerGo != null)
            {
                playerGo.SetActive(true);
                playerGo.transform.position = _revivePosition + Vector3.up * 1f;

                if (playerStats != null)
                {
                    playerStats.Health.Reset();
                    playerStats.ActivateImmortality(3f);
                }
                if (player != null)
                    player.ClearDeathCause();
            }

            _reviveUsed = true;
        }
    }
}

using System;
using System.Collections;
using _Scripts.CoreSystem;
using _Scripts.GooglePlay;
using _Scripts.Managers.Lava_Logic;
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
        [SerializeField] private GameObject pausePanel;
        
        private AdsManager adsManager;
        private LavaManager lavaManager;
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
            if (lavaManager == null)
            {
                var lavaObj = GameObject.Find("LavaManager");
                if (lavaObj != null)
                    lavaManager = lavaObj.GetComponent<LavaManager>();
            }
        }

        public void TriggerGameOver()
        {
            if (playerGo != null)
                _revivePosition = playerGo.transform.position;
            lastDeathWasLava = player.DiedByLava;
            Debug.Log("player died by lava true or false: " + lastDeathWasLava);
            StartCoroutine(ShowGameOverWithDelay());
        }

        private IEnumerator ShowGameOverWithDelay()
        {
            yield return new WaitForSeconds(1); // Wait for death animation
            gameOverUI.SetActive(true);
            
            if (lastDeathWasLava || _reviveUsed)
            {
                reviveButton.gameObject.SetActive(false);
            }
            else
            {
                reviveButton.gameObject.SetActive(true);
            }

            Time.timeScale = 0;
            
        }

        private void OnReviveButtonClicked()
        {
            if (_reviveUsed || lastDeathWasLava) return;
            
            // stay paused while ad is showing
            var rewardEarned = false;
            
            var shown = adsManager.ShowRewarded(
                onReward: r => { rewardEarned = true; },     // do not touch timeScale here
                onClosed: () =>
                {
                    if (rewardEarned)
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
                        lavaManager.ActivateLavaDelay(15f);
            
                        _reviveUsed = true;

                        StartCoroutine(PauseGame(1f));
                    }
                    else
                    {
                        // ad closed without reward, keep game over and paused
                        gameOverUI.SetActive(true);
                    }
                });
            
            if (!shown)
            {
                Debug.Log("Rewarded ad not ready");
            }
        }

        private IEnumerator PauseGame(float time)
        {
            yield return new WaitForSeconds(time);
            pausePanel.SetActive(true);
            Time.timeScale = 0;
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
            lavaManager.ActivateLavaDelay(15f);
            
            _reviveUsed = true;
        }
    }
}

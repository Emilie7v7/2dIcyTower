using System;
using System.Collections;
using _Scripts.CoreSystem;
using _Scripts.GooglePlay;
using _Scripts.InputHandler;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.Managers.Lava_Logic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.Managers.GameOver_Logic
{
    public class GameOverManager : MonoBehaviour
    {
        public static GameOverManager Instance { get; private set; }
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button doubleCoinsButton;
        [SerializeField] private PlayerComponent.Player player;
        [SerializeField] private Stats playerStats;
        [SerializeField] private GameObject playerGo;
        [SerializeField] private GameObject pausePanel;
        
        private AdsManager adsManager;
        private LavaManager lavaManager;
        private bool _reviveUsed = false;
        private bool lastDeathWasLava = false;
        private Vector3 _revivePosition;
        private bool _doubleCoinsUsed = false;
        
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

            if (doubleCoinsButton != null)
            {
                doubleCoinsButton.onClick.AddListener(OnDoubleCoinsButtonClicked);
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
            
            if (lastDeathWasLava || _reviveUsed || _doubleCoinsUsed)
            {
                reviveButton.gameObject.SetActive(false);
            }
            else
            {
                reviveButton.gameObject.SetActive(true);
            }

            doubleCoinsButton.gameObject.SetActive(!_doubleCoinsUsed);

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

                        // Fix touch input AFTER game is resumed and everything is active
                        StartCoroutine(FixTouchInputAfterAd());

                        //StartCoroutine(PauseGame(0.5f));
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
        
        private void OnDoubleCoinsButtonClicked()
        {
            if (_doubleCoinsUsed) return;

            var rewardEarned = false;
            
            var shown = adsManager.ShowRewarded(
                onReward: r => { rewardEarned = true; },
                onClosed: () =>
                {
                    if (rewardEarned)
                    {
                        GameManager.Instance.DoubleCoinsFromLastGame();
                        _doubleCoinsUsed = true;
                        
                        doubleCoinsButton.gameObject.SetActive(false);

                        StartCoroutine(FreezeGameAfterAd());
                    }
                });
            
            if (!shown)
            {
                Debug.Log("Rewarded ad not ready");
            }
        }
        
        private void PauseGame()
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        private IEnumerator FreezeGameAfterAd()
        {
            yield return new WaitForEndOfFrame();
            Time.timeScale = 0;
        }
        
        private IEnumerator FixTouchInputAfterAd()
        {
            // Wait for the game to fully resume
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            
            Debug.Log("Attempting to fix touch input after ad...");
            
            // Method 1: Reset EnhancedTouchSupport
            bool touchSupportReset = false;
            try
            {
                EnhancedTouchSupport.Disable();
                touchSupportReset = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"EnhancedTouchSupport.Disable() failed: {e.Message}");
            }
            
            if (touchSupportReset)
            {
                yield return new WaitForEndOfFrame();
                try
                {
                    EnhancedTouchSupport.Enable();
                    Debug.Log("EnhancedTouchSupport reset completed");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"EnhancedTouchSupport.Enable() failed: {e.Message}");
                }
            }
            
            // Method 2: Reset the touchscreen device
            yield return new WaitForEndOfFrame();
            try
            {
                var touchscreen = Touchscreen.current;
                if (touchscreen != null)
                {
                    InputSystem.ResetDevice(touchscreen);
                    Debug.Log("Touchscreen device reset completed");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Touchscreen reset failed: {e.Message}");
            }
            
            // Method 3: Reset PlayerInputHandler
            yield return new WaitForEndOfFrame();
            var inputHandler = playerGo != null ? playerGo.GetComponent<PlayerInputHandler>() : null;
            
            if (inputHandler != null)
            {
                // Reset PlayerInput component
                var playerInput = inputHandler.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    playerInput.enabled = false;
                    yield return new WaitForEndOfFrame();
                    playerInput.enabled = true;
                    Debug.Log("PlayerInput component reset completed");
                }
                
                // Reset the handler itself
                inputHandler.enabled = false;
                yield return new WaitForEndOfFrame();
                inputHandler.enabled = true;
                Debug.Log("PlayerInputHandler reset completed");
            }
            
            Debug.Log("Touch input fix sequence completed");
            
            yield return new WaitForEndOfFrame();
            PauseGame();
        }
        
    }
}

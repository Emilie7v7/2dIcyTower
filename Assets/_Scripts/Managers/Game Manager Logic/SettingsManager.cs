using System.IO;
using _Scripts.Generics;
using _Scripts.JSON;
using _Scripts.Managers.Audio_Logic;
using _Scripts.Managers.Save_System_Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers.Game_Manager_Logic
{
    public static class SettingsManager
    {
        private static int _cachedMaxRefreshRate = -1;
        public static OptionsData OptionsData { get; private set; }

        #region Fps
        private static GameObject _incompatibilityPanel;
        private static TextMeshProUGUI _incompatibilityText; 
        private static TMP_Dropdown _tmpDropdownForFps;
        private static Slider _fpsSlider;
        #endregion
        
        static SettingsManager()
        {
            LoadOptionsData();
        }

        #region Options Game Data
        public static void SaveOptionsData()
        {
            SaveSystem.SaveOptionsData(OptionsData);
        }

        public static void LoadOptionsData()
        {
            if (File.Exists(SaveSystem.OptionsDataSavePath))
            {
                OptionsData = SaveSystem.LoadOptionsData() ?? new OptionsData();
            }
            else
            {
                Debug.Log("No Options File Found, Creating Default...");
                OptionsData = new OptionsData();
                SaveOptionsData();
            }
            ApplySettings();
        }

        public static void ResetOptionsData()
        {
            SaveSystem.DeleteOptionsData();
            OptionsData = new OptionsData();
            SaveOptionsData();
            LoadOptionsData();
        }

        public static void ApplyAndSaveSettings(OptionsData newSettings)
        {
            OptionsData = newSettings;
            SaveOptionsData();
            ApplyAllSettings();
        }
        #endregion

        private static void ApplySettings()
        {
            QualitySettings.vSyncCount = OptionsData.vSync ? 1 : 0;
            Application.targetFrameRate = OptionsData.fpsCount;
        }

        private static void ApplyAllSettings()
        {
            // Apply FPS settings
            ApplySettings();
            
            // Apply FPS Counter visibility
            FPSCounter.SetVisibility(OptionsData.showFps);
            
            // Apply Audio Settings
            AudioManager.Instance.SetVolume(OptionsData.musicVolume, AudioManager.AudioTypes.Music);
            AudioManager.Instance.SetVolume(OptionsData.sfxVolume, AudioManager.AudioTypes.SFX);
            AudioManager.Instance.SetVolume(OptionsData.uiVolume, AudioManager.AudioTypes.UI);
        }

        #region Controls Settings
        public static OptionsData.ControlModes GetControlMode()
        {
            return OptionsData.controlMode;
        }

        public static void ApplyControlMode(OptionsData.ControlModes mode)
        {
            OptionsData.controlMode = mode;
            SaveOptionsData();
        }
        #endregion
        
        #region Fps Settings
        public static void ApplyFPS(int requestedFps)
        {
            QualitySettings.vSyncCount = 0; // First disable VSync
            var maxFps = GetMaxRefreshRateForMobile();
    
            int targetFps;
            if (requestedFps > maxFps)
            {
                targetFps = maxFps;
                ShowIncompatibilityErrorFpsMessage(requestedFps, targetFps);
            }
            else
            {
                targetFps = requestedFps;
            }

            Debug.Log($"Setting FPS to {targetFps} (Requested: {requestedFps}, Device Max: {maxFps})");
    
            OptionsData.fpsCount = targetFps;
            OptionsData.vSync = false;
            SaveOptionsData();
            Application.targetFrameRate = targetFps;
        }



        public static void ApplyShowFPS(bool show)
        {
            OptionsData.showFps = show;
            SaveOptionsData();
            FPSCounter.SetVisibility(show);
        }
        
        private static int GetMaxRefreshRateForMobile()
        {
            if (_cachedMaxRefreshRate == -1)
            {
                // Try to get from screen, explicitly cast double to float
                float refreshRate = (float)Screen.currentResolution.refreshRateRatio.value;
        
                // Fallback values if we get an invalid refresh rate
                if (refreshRate <= 0)
                {
#if UNITY_ANDROID
                    try 
                    {
                        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                        {
                            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                            {
                                using (var windowManager = activity.Call<AndroidJavaObject>("getWindowManager"))
                                {
                                    using (var display = windowManager.Call<AndroidJavaObject>("getDefaultDisplay"))
                                    {
                                        refreshRate = display.Call<float>("getRefreshRate");
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"Failed to get refresh rate from Android API: {e.Message}");
                        refreshRate = 60f; // Fallback to 60 if Android API call fails
                    }
#endif
                }
        
                _cachedMaxRefreshRate = Mathf.RoundToInt(refreshRate);
        
                // Fallback to 60 if we still don't have a valid value
                if (_cachedMaxRefreshRate <= 0)
                    _cachedMaxRefreshRate = 60;
            
                Debug.Log($"Detected max refresh rate: {_cachedMaxRefreshRate}Hz");
            }
    
            return _cachedMaxRefreshRate;
        }

        
        public static void InitializeIncompatibilityPanel(GameObject panel, TextMeshProUGUI text, TMP_Dropdown dropdown, Slider slider)
        {
            _incompatibilityPanel = panel;
            _incompatibilityText = text;
            _tmpDropdownForFps = dropdown;
            _fpsSlider = slider;
        }

        private static void ShowIncompatibilityErrorFpsMessage(int fps, int maxFps)
        {
            if (_incompatibilityPanel != null && _incompatibilityText != null) 
            {
                _incompatibilityPanel.SetActive(true);
                _incompatibilityText.text = $"{fps} FPS is not supported on your device! Resetting to {maxFps} FPS.";
                _tmpDropdownForFps.value = 0;
                _fpsSlider.value = 0;
                _tmpDropdownForFps.RefreshShownValue();
            }
        }
        #endregion
        
        #region Vsync Settings
        public static void ApplyVSync(bool enabled)
        {
            OptionsData.vSync = enabled;
            SaveOptionsData();
            ApplySettings();
        }
        #endregion

        #region Music Settings
        public static float GetMusicVolume()
        {
            return OptionsData.musicVolume;
        }

        public static void ApplyMusicVolume(float volume)
        {
            OptionsData.musicVolume = volume;
            SaveOptionsData();
            AudioManager.Instance.SetVolume(volume, AudioManager.AudioTypes.Music);
        }
        
        public static float GetSfxVolume()
        {
            return OptionsData.sfxVolume;
        }

        public static void ApplySfxVolume(float volume)
        {
            OptionsData.sfxVolume = volume;
            SaveOptionsData();
            AudioManager.Instance.SetVolume(volume, AudioManager.AudioTypes.SFX);
        }
        
        public static float GetUiVolume()
        {
            return OptionsData.uiVolume;
        }

        public static void ApplyUiVolume(float volume)
        {
            OptionsData.uiVolume = volume;
            SaveOptionsData();
            AudioManager.Instance.SetVolume(volume, AudioManager.AudioTypes.UI);
        }
        #endregion
    }
}
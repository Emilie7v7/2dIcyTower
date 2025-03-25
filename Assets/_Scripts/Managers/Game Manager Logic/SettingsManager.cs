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
                //Debug.Log("Options Loaded Successfully!");
            }
            else
            {
                Debug.Log("No Options File Found, Creating Default...");
                OptionsData = new OptionsData();
                SaveOptionsData(); // Ensure it's created
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
        #endregion

        private static void ApplySettings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = OptionsData.fpsCount;
        }

        #region Controls Settings

        public static int GetControlMode()
        {
            return OptionsData.controlMode;
        }

        public static void SetControlMode(int mode)
        {
            OptionsData.controlMode = mode;
            SaveOptionsData();
        }

        #endregion
        
        #region Fps Settings

        public static void SetFPS(int fps)
        {
            var maxFps = GetMaxRefreshRateForMobile();

            if (fps > maxFps)
            {
                ShowIncompatibilityErrorFpsMessage(fps, maxFps);
                fps = maxFps;
            }
            
            OptionsData.fpsCount = fps;
            SaveOptionsData();
    
            Application.targetFrameRate = fps;
        }

        public static void SetShowFPS(bool show)
        {
            OptionsData.showFps = show;
            SaveOptionsData();
            FPSCounter.SetVisibility(show);
        }
        
        private static int GetMaxRefreshRateForMobile()
        {
            return Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
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
        
        public static void SetVSync(bool enabled)
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

        public static void SetMusicVolume(float volume)
        {
            OptionsData.musicVolume = volume;
            SaveOptionsData();
            
            AudioManager.Instance.SetVolume(volume, AudioManager.AudioTypes.Music);
        }
        
        public static float GetSfxVolume()
        {
            return OptionsData.sfxVolume;
        }

        public static void SetSfxVolume(float volume)
        {
            OptionsData.sfxVolume = volume;
            SaveOptionsData();
            
            AudioManager.Instance.SetVolume(volume, AudioManager.AudioTypes.SFX);
        }
        
        public static float GetUiVolume()
        {
            return OptionsData.uiVolume;
        }
        public static void SetUiVolume(float volume)
        {
            OptionsData.uiVolume = volume;
            SaveOptionsData();
            
            AudioManager.Instance.SetVolume(volume, AudioManager.AudioTypes.UI);
        }
        
        #endregion
    }
}

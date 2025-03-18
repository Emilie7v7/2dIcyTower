using System.IO;
using _Scripts.Generics;
using _Scripts.JSON;
using _Scripts.Managers.Save_System_Logic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Scripts.Managers.Game_Manager_Logic
{
    public static class SettingsManager
    {
        public static OptionsData OptionsData { get; private set; }

        private static GameObject _incompatibilityPanel;
        private static TextMeshProUGUI _incompatibilityText; 
        private static TMP_Dropdown _tmpDropdownForFps;
        
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
                Debug.Log("Options Loaded Successfully!");
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

        public static void ApplySettings()
        {
            QualitySettings.vSyncCount = OptionsData.vSync ? 1 : 0;
            Application.targetFrameRate = OptionsData.fpsCount; // Apply FPS cap **after** VSync
    
            Debug.Log($"Settings Applied: FPS = {OptionsData.fpsCount}, VSync = {(OptionsData.vSync ? "Enabled" : "Disabled")}");

            //Force a Quality Settings Refresh to ensure Unity updates VSync
            QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel(), true);
        }

        public static int GetControlMode()
        {
            return OptionsData.controlMode;
        }

        public static void SetControlMode(int mode)
        {
            OptionsData.controlMode = mode;
            SaveOptionsData();
        }
        
        // UI Methods to update settings dynamically
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
            ApplySettings();
        }

        public static void SetShowFPS(bool show)
        {
            OptionsData.showFps = show;
            SaveOptionsData();
            FPSCounter.SetVisibility(show);
        }
        
        public static void SetVSync(bool enabled)
        {
            OptionsData.vSync = enabled;
            SaveOptionsData();
            ApplySettings();
        }

        private static int GetMaxRefreshRateForMobile()
        {
            return Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
        }


        public static void InitializeIncompatibilityPanel(GameObject panel, TextMeshProUGUI text, TMP_Dropdown dropdown)
        {
            _incompatibilityPanel = panel;
            _incompatibilityText = text;
            _tmpDropdownForFps = dropdown;
        }
        private static void ShowIncompatibilityErrorFpsMessage(int fps, int maxFps)
        {
            if (_incompatibilityPanel != null && _incompatibilityText != null) 
            {
                _incompatibilityPanel.SetActive(true);
                _incompatibilityText.text = $"{fps} FPS is not supported on your device! Resetting to {maxFps} FPS.";
                _tmpDropdownForFps.value = 0;
                _tmpDropdownForFps.RefreshShownValue();
            }
        }
    }
}

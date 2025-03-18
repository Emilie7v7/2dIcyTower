using System;
using _Scripts.Managers.Game_Manager_Logic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Scripts.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Fps Settings")]
        [SerializeField] private TMP_Dropdown fpsDropdown;
        [SerializeField] private GameObject incompatibilityPanel;
        [SerializeField] private TextMeshProUGUI incompatibilityText;
        [SerializeField] private Toggle  showFpsToggle;
        
        [Header("Other Settings")]
        [SerializeField] private Toggle vsyncToggle;
        [Header("Control Settings")]
        [SerializeField] private TMP_Dropdown controlDropdown;

        private void Start()
        {
            fpsDropdown.value = GetFPSDropdownIndex(SettingsManager.OptionsData.fpsCount);
            showFpsToggle.isOn = SettingsManager.OptionsData.showFps;
            
            vsyncToggle.isOn = SettingsManager.OptionsData.vSync;
            
            fpsDropdown.onValueChanged.AddListener(SetFPS);
            showFpsToggle.onValueChanged.AddListener(SetShowFps);
            
            vsyncToggle.onValueChanged.AddListener(SetVSync);
            
            controlDropdown.onValueChanged.AddListener(SetControlMode);
            
            LoadControlMode();
            SettingsManager.InitializeIncompatibilityPanel(incompatibilityPanel, incompatibilityText, fpsDropdown);
            incompatibilityPanel.SetActive(false);
        }   
        
        #region FPS Settings
        
        private static void SetFPS(int index)
        {
            var fps = GetFPSValueFromDropdown(index);
            SettingsManager.SetFPS(fps);
        }
        private static void SetShowFps(bool show)
        {
            SettingsManager.SetShowFPS(show);
        }
        
        private static int GetFPSDropdownIndex(int fps)
        {
            var maxFps = GetMaxRefreshRateForMobile();
            
            switch (fps)
            {
                case 60: return 0;
                case 90 when maxFps >= 90: return 1;
                case 120 when maxFps >= 120: return 2;
                default: return 0;
            }
        }
        private static int GetFPSValueFromDropdown(int index)
        {
            switch (index)
            {
                case 0: return 60;
                case 1: return 90;
                case 2: return 120;
                default: return 60;
            }
        }

        private static int GetMaxRefreshRateForMobile()
        {
            return Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
        }
        
        #endregion
        
        #region Vsync Settings
        
        private static void SetVSync(bool value)
        {
            SettingsManager.SetVSync(value);
        }
        #endregion

        private static void SetControlMode(int mode)
        {
            SettingsManager.SetControlMode(mode);
        }

        private void LoadControlMode()
        {
            controlDropdown.value = SettingsManager.GetControlMode();
        }
    }
}

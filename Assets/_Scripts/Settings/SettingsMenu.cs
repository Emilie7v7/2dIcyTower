using System;
using _Scripts.JSON;
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
        [SerializeField] private Slider fpsSlider;
        [SerializeField] private GameObject incompatibilityPanel;
        [SerializeField] private TextMeshProUGUI incompatibilityText;
        [SerializeField] private Toggle  showFpsToggle;
        
        [Header("Other Settings")]
        [SerializeField] private Toggle vsyncToggle;
        
        [Header("Control Settings")]
        [SerializeField] private Toggle touchScreenToggle;
        [SerializeField] private Toggle joystickToggle;
        
        [Header("Music Settings")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider uiSoundVolumeSlider;

        private void Start()
        {
            #region Fps

            fpsDropdown.value = GetFPSSliderIndex(SettingsManager.OptionsData.fpsCount);
            fpsSlider.value = GetFPSSliderIndex(SettingsManager.OptionsData.fpsCount);
            showFpsToggle.isOn = SettingsManager.OptionsData.showFps;
            
            fpsDropdown.onValueChanged.AddListener(SetFPS);
            fpsSlider.onValueChanged.AddListener(value => SetFPS((int)value));
            showFpsToggle.onValueChanged.AddListener(SetShowFps);

            #endregion
            
            #region Vsync
            vsyncToggle.isOn = SettingsManager.OptionsData.vSync;
            
            vsyncToggle.onValueChanged.AddListener(SetVSync);
            
            #endregion

            #region Controls

            var controlMode = SettingsManager.GetControlMode();
            touchScreenToggle.isOn = controlMode == OptionsData.ControlModes.Touchscreen;
            joystickToggle.isOn = controlMode == OptionsData.ControlModes.Joystick;

            LoadControlMode();
            
            #endregion

            #region Music

            musicVolumeSlider.value = SettingsManager.GetMusicVolume();
            sfxVolumeSlider.value = SettingsManager.GetSfxVolume();
            uiSoundVolumeSlider.value = SettingsManager.GetUiVolume();
            
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
            uiSoundVolumeSlider.onValueChanged.AddListener(SetUiVolume);
            
            #endregion
            SettingsManager.InitializeIncompatibilityPanel(incompatibilityPanel, incompatibilityText, fpsDropdown, fpsSlider);
            incompatibilityPanel.SetActive(false);
            
        }   
        
        #region FPS Settings
        
        private static void SetFPS(int index)
        {
            var fps = GetFPSValueFromSlider(index);
            SettingsManager.SetFPS(fps);
        }
        private static void SetShowFps(bool show)
        {
            SettingsManager.SetShowFPS(show);
        }
        
        private static int GetFPSSliderIndex(int fps)
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
        private static int GetFPSValueFromSlider(int index)
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

        #region Controls Settings

        private void LoadControlMode()
        {
            touchScreenToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    joystickToggle.isOn = false;
                    SettingsManager.SetControlMode(OptionsData.ControlModes.Touchscreen);
                }
            });

            joystickToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    touchScreenToggle.isOn = false;
                    SettingsManager.SetControlMode(OptionsData.ControlModes.Joystick);
                }
            });
        }
        

        #endregion
        
        #region Music Settings

        private static void SetMusicVolume(float volume)
        {
            SettingsManager.SetMusicVolume(volume);
        }

        private static void SetSfxVolume(float volume)
        {
            SettingsManager.SetSfxVolume(volume);
        }

        private static void SetUiVolume(float volume)
        {
            SettingsManager.SetUiVolume(volume);
        }
        
        #endregion
    }
}

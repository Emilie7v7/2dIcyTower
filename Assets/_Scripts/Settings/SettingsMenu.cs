using _Scripts.JSON;
using _Scripts.Managers.Game_Manager_Logic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Scripts.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Menu References")]
        [SerializeField] private GameObject mainMenuPanel;

        [Header("Fps Settings")]
        [SerializeField] private TMP_Dropdown fpsDropdown;
        [SerializeField] private Slider fpsSlider;
        [SerializeField] private GameObject incompatibilityPanel;
        [SerializeField] private TextMeshProUGUI incompatibilityText;
        [SerializeField] private Toggle showFpsToggle;
        
        [Header("Other Settings")]
        [SerializeField] private Toggle vsyncToggle;
        
        [Header("Control Settings")]
        [SerializeField] private Toggle touchScreenToggle;
        [SerializeField] private Toggle joystickToggle;
        
        [Header("Music Settings")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider uiSoundVolumeSlider;
        
        [Header("UI Elements")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject unsavedChangesPanel;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private PendingSettings _pendingSettings;

        private void Start()
        {
            _pendingSettings = new PendingSettings(SettingsManager.OptionsData);
            SetupUI();
            LoadInitialSettings();
            SettingsManager.InitializeIncompatibilityPanel(incompatibilityPanel, incompatibilityText, fpsDropdown, fpsSlider);
        }

        // Modify the Apply button listener setup in SetupUI
        private void SetupUI()
        {
            applyButton.onClick.AddListener(ApplySettings);
            closeButton.onClick.AddListener(OnCloseButtonClick);
            confirmButton.onClick.AddListener(OnConfirmExit);
            cancelButton.onClick.AddListener(OnCancelExit);
        
            applyButton.interactable = false;
            unsavedChangesPanel.SetActive(false);
            incompatibilityPanel.SetActive(false);
        }


        private void LoadInitialSettings()
        {
            #region FPS Settings
            fpsDropdown.value = GetFPSSliderIndex(_pendingSettings.TempOptionsData.fpsCount);
            fpsSlider.value = GetFPSSliderIndex(_pendingSettings.TempOptionsData.fpsCount);
            showFpsToggle.isOn = _pendingSettings.TempOptionsData.showFps;
            
            fpsDropdown.onValueChanged.AddListener(OnFPSChanged);
            fpsSlider.onValueChanged.AddListener(value => OnFPSChanged((int)value));
            showFpsToggle.onValueChanged.AddListener(OnShowFpsChanged);
            #endregion

            #region VSync Settings
            vsyncToggle.isOn = _pendingSettings.TempOptionsData.vSync;
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
            #endregion

            #region Control Settings
            var controlMode = _pendingSettings.TempOptionsData.controlMode;
            touchScreenToggle.isOn = controlMode == OptionsData.ControlModes.Touchscreen;
            joystickToggle.isOn = controlMode == OptionsData.ControlModes.Joystick;
            
            SetupControlToggles();
            UpdateToggleInteractableStates();
            #endregion

            #region Audio Settings
            musicVolumeSlider.value = _pendingSettings.TempOptionsData.musicVolume;
            sfxVolumeSlider.value = _pendingSettings.TempOptionsData.sfxVolume;
            uiSoundVolumeSlider.value = _pendingSettings.TempOptionsData.uiVolume;
            
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            uiSoundVolumeSlider.onValueChanged.AddListener(OnUiVolumeChanged);
            #endregion
        }

        #region Settings Change Handlers
        private void OnFPSChanged(int index)
        {
            var fps = GetFPSValueFromSlider(index);
            _pendingSettings.TempOptionsData.fpsCount = fps;
            MarkAsModified();
        }

        private void OnShowFpsChanged(bool show)
        {
            _pendingSettings.TempOptionsData.showFps = show;
            MarkAsModified();
        }

        private void OnVSyncChanged(bool enable)
        {
            _pendingSettings.TempOptionsData.vSync = enable;
            MarkAsModified();
        }

        private void OnMusicVolumeChanged(float volume)
        {
            _pendingSettings.TempOptionsData.musicVolume = volume;
            MarkAsModified();
        }

        private void OnSfxVolumeChanged(float volume)
        {
            _pendingSettings.TempOptionsData.sfxVolume = volume;
            MarkAsModified();
        }

        private void OnUiVolumeChanged(float volume)
        {
            _pendingSettings.TempOptionsData.uiVolume = volume;
            MarkAsModified();
        }
        #endregion

        #region Control Settings
        private void SetupControlToggles()
        {
            touchScreenToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    _pendingSettings.TempOptionsData.controlMode = OptionsData.ControlModes.Touchscreen;
                    joystickToggle.isOn = false;
                    UpdateToggleInteractableStates();
                    MarkAsModified();
                }
            });

            joystickToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    _pendingSettings.TempOptionsData.controlMode = OptionsData.ControlModes.Joystick;
                    touchScreenToggle.isOn = false;
                    UpdateToggleInteractableStates();
                    MarkAsModified();
                }
            });
        }

        private void UpdateToggleInteractableStates()
        {
            touchScreenToggle.interactable = !touchScreenToggle.isOn;
            joystickToggle.interactable = !joystickToggle.isOn;
        }
        #endregion

        #region FPS Utility Methods
        private static int GetFPSSliderIndex(int fps)
        {
            var maxFps = GetMaxRefreshRateForMobile();
            return fps switch
            {
                60 => 0,
                90 when maxFps >= 90 => 1,
                120 when maxFps >= 120 => 2,
                _ => 0
            };
        }

        private static int GetFPSValueFromSlider(int index)
        {
            return index switch
            {
                0 => 60,
                1 => 90,
                2 => 120,
                _ => 60
            };
        }

        private static int GetMaxRefreshRateForMobile()
        {
            return Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
        }
        #endregion

        #region Menu Navigation
        private void ApplySettings()
        {
            SettingsManager.ApplyAndSaveSettings(_pendingSettings.TempOptionsData);
            _pendingSettings.IsModified = false;
            applyButton.interactable = false;
        }


        private void OnCloseButtonClick()
        {
            if (_pendingSettings.IsModified)
            {
                unsavedChangesPanel.SetActive(true);
            }
            else
            {
                CloseMenu();
            }
        }


        private void OnConfirmExit()
        {
            ApplySettings();
            CloseMenu();
        }

        private void OnCancelExit()
        {
            _pendingSettings = new PendingSettings(SettingsManager.OptionsData);
            LoadInitialSettings();
            CloseMenu();
        }

        private void CloseMenu()
        {
            unsavedChangesPanel.SetActive(false);
            gameObject.SetActive(false);
            _pendingSettings = new PendingSettings(SettingsManager.OptionsData);
        
            // Show the main menu panel when closing settings
            if (mainMenuPanel)
            {
                mainMenuPanel.SetActive(true);
            }
        }

        private void MarkAsModified()
        {
            _pendingSettings.IsModified = true;
            applyButton.interactable = true;
        }

        #endregion
    }

    public class PendingSettings
    {
        public bool IsModified { get; set; }
        public OptionsData TempOptionsData { get; }

        public PendingSettings(OptionsData currentSettings)
        {
            IsModified = false;
            TempOptionsData = new OptionsData
            {
                fpsCount = currentSettings.fpsCount,
                showFps = currentSettings.showFps,
                vSync = currentSettings.vSync,
                controlMode = currentSettings.controlMode,
                musicVolume = currentSettings.musicVolume,
                sfxVolume = currentSettings.sfxVolume,
                uiVolume = currentSettings.uiVolume
            };
        }
    }
}

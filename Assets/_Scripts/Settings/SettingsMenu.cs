using System.Collections;
using _Scripts.JSON;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.Managers.Save_System_Logic;
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

        private void Awake()
        {
            // Pre-initialize the toggle states
            var options = SaveSystem.LoadOptionsData();
    
            if (touchScreenToggle) touchScreenToggle.isOn = (options.controlMode == OptionsData.ControlModes.Touchscreen);
            if (joystickToggle) joystickToggle.isOn = (options.controlMode == OptionsData.ControlModes.Joystick);
        }

        private void OnEnable()
        {
            InitializeSettings();    
            StartCoroutine(UpdateControlModeNextFrame());
        }

        private void Start()
        {
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            _pendingSettings = new PendingSettings(SettingsManager.OptionsData);
            SetupUI();
            
            // First set control states without notification to prevent flicker
            PreInitializeControlStates();
            
            // Then load all settings
            LoadInitialSettings();
            
            SettingsManager.InitializeIncompatibilityPanel(incompatibilityPanel, incompatibilityText, fpsDropdown, fpsSlider);
        }

        private void PreInitializeControlStates()
        {
            var controlMode = _pendingSettings.TempOptionsData.controlMode;
            touchScreenToggle.SetIsOnWithoutNotify(controlMode == OptionsData.ControlModes.Touchscreen);
            joystickToggle.SetIsOnWithoutNotify(controlMode == OptionsData.ControlModes.Joystick);
            
            UpdateToggleInteractableStates();
        }

        public void UpdateControlMode()
        {
            if (!gameObject.activeSelf) return; // Don't update if inactive
    
            var options = SaveSystem.LoadOptionsData();
            Debug.Log($"Updating control mode to: {options.controlMode}");
    
            touchScreenToggle.SetIsOnWithoutNotify(options.controlMode == OptionsData.ControlModes.Touchscreen);
            joystickToggle.SetIsOnWithoutNotify(options.controlMode == OptionsData.ControlModes.Joystick);
    
            UpdateToggleInteractableStates();
    
            if (_pendingSettings != null)
            {
                _pendingSettings.TempOptionsData.controlMode = options.controlMode;
            }
        }


        private IEnumerator UpdateControlModeNextFrame()
        {
            yield return null; // Wait for the next frame
            UpdateControlMode();
        }

        private void SetupUI()
        {
            RemoveAllListeners();

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
            int currentFpsIndex = GetFPSSliderIndex(_pendingSettings.TempOptionsData.fpsCount);
            fpsDropdown.value = currentFpsIndex;
            fpsSlider.value = currentFpsIndex;
            
            fpsDropdown.onValueChanged.AddListener(OnFPSChanged);
            fpsSlider.onValueChanged.AddListener(value => OnFPSChanged((int)value));
            
            showFpsToggle.isOn = _pendingSettings.TempOptionsData.showFps;
            showFpsToggle.onValueChanged.AddListener(OnShowFpsChanged);
            #endregion

            #region VSync Settings
            vsyncToggle.isOn = _pendingSettings.TempOptionsData.vSync;
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
            #endregion

            #region Control Settings
            SetupControlToggles();
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

        private void RemoveAllListeners()
        {
            applyButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            
            fpsDropdown.onValueChanged.RemoveAllListeners();
            fpsSlider.onValueChanged.RemoveAllListeners();
            showFpsToggle.onValueChanged.RemoveAllListeners();
            vsyncToggle.onValueChanged.RemoveAllListeners();
            touchScreenToggle.onValueChanged.RemoveAllListeners();
            joystickToggle.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            uiSoundVolumeSlider.onValueChanged.RemoveAllListeners();
        }

        #region Settings Change Handlers
        private void OnFPSChanged(int index)
        {
            var fps = GetFPSValueFromSlider(index);
            if (fpsDropdown.value != index) fpsDropdown.value = index;
            if (!Mathf.Approximately(fpsSlider.value, index)) fpsSlider.value = index;
            
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
                    joystickToggle.SetIsOnWithoutNotify(false);
                    UpdateToggleInteractableStates();
                    MarkAsModified();
                }
            });

            joystickToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    _pendingSettings.TempOptionsData.controlMode = OptionsData.ControlModes.Joystick;
                    touchScreenToggle.SetIsOnWithoutNotify(false);
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
            return fps switch
            {
                60 => 0,
                90 => 1,
                120 => 2,
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
        #endregion

        #region Menu Navigation
        private void ApplySettings()
        {
            int targetFps = _pendingSettings.TempOptionsData.fpsCount;
            SettingsManager.ApplyFPS(targetFps);

            SettingsManager.ApplyShowFPS(_pendingSettings.TempOptionsData.showFps);
            SettingsManager.ApplyVSync(_pendingSettings.TempOptionsData.vSync);
            SettingsManager.ApplyControlMode(_pendingSettings.TempOptionsData.controlMode);
            SettingsManager.ApplyMusicVolume(_pendingSettings.TempOptionsData.musicVolume);
            SettingsManager.ApplySfxVolume(_pendingSettings.TempOptionsData.sfxVolume);
            SettingsManager.ApplyUiVolume(_pendingSettings.TempOptionsData.uiVolume);

            _pendingSettings.TempOptionsData.fpsCount = SettingsManager.OptionsData.fpsCount;
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
        
            if (mainMenuPanel != null)
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

        private void OnDisable()
        {
            RemoveAllListeners();
        }
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
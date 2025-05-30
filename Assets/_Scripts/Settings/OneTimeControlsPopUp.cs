using System;
using _Scripts.JSON;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.Managers.Save_System_Logic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Settings
{
    public class OneTimeControlsPopUp : MonoBehaviour
    {
        [SerializeField] private SettingsMenu settingsMenu;
        [SerializeField] private Toggle oneTimePopUpToggle;
        [SerializeField] private GameObject controlsPopUp;
        
        [Header("Control Settings")]
        [SerializeField] private Toggle touchScreenToggle;
        [SerializeField] private Toggle joystickToggle;

        
        private void OnEnable()
        {
            InitializeSettings();
        }
        
        private void Start()
        {
            var options = SaveSystem.LoadOptionsData();

            if (options.hideControlsPopUp)
            {
                gameObject.SetActive(false);
                return;
            }
            
            controlsPopUp.SetActive(true);
        }

        private void InitializeSettings()
        {
            SetupUI();
            PreInitializeControlStates();
            LoadInitialSettings();
        }
        
        private void SetupUI()
        {
            RemoveAllListeners();
        }
        private void LoadInitialSettings()
        {
            SetupControlToggles();
        }
        private void PreInitializeControlStates()
        {
            var options = SaveSystem.LoadOptionsData();
            touchScreenToggle.SetIsOnWithoutNotify(options.controlMode == OptionsData.ControlModes.Touchscreen);
            joystickToggle.SetIsOnWithoutNotify(options.controlMode == OptionsData.ControlModes.Joystick);
            
            UpdateToggleInteractableStates();
        }
        private void RemoveAllListeners()
        {
            touchScreenToggle.onValueChanged.RemoveAllListeners();
            joystickToggle.onValueChanged.RemoveAllListeners();
        }
        public void OnApplyButton()
        {
            var options = SaveSystem.LoadOptionsData();
    
            options.hideControlsPopUp = oneTimePopUpToggle.isOn;
            options.controlMode = touchScreenToggle.isOn ? 
                OptionsData.ControlModes.Touchscreen : 
                OptionsData.ControlModes.Joystick;
    
            SaveSystem.SaveOptionsData(options);
    
            // Don't try to update the settings menu UI here
            gameObject.SetActive(false);
        }

        
        private void SetupControlToggles()
        {
            var options = SaveSystem.LoadOptionsData();
            touchScreenToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    joystickToggle.SetIsOnWithoutNotify(false);
                    UpdateToggleInteractableStates();
                }
            });

            joystickToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    touchScreenToggle.SetIsOnWithoutNotify(false);
                    UpdateToggleInteractableStates();
                }
            });
        }

        private void UpdateToggleInteractableStates()
        {
            touchScreenToggle.interactable = !touchScreenToggle.isOn;
            joystickToggle.interactable = !joystickToggle.isOn;
        }
        
    }
    
}

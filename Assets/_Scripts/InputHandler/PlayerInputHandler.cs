using System;
using System.Collections;
using _Scripts.Managers.Game_Manager_Logic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace _Scripts.InputHandler
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public PlayerInput playerInput;
        private ControlMode _controlMode = ControlMode.TapToThrow;
        
        [SerializeField] private VirtualJoystick aimingJoystick;
        [SerializeField] private GameObject throwButton;
        
        private static UnityEngine.Camera MainCamera => UnityEngine.Camera.main;
        
        public Vector2 RawThrowDirection { get; private set; }
        public Vector2 ThrowDirectionInput { get; private set; }
        public Vector2 RawMovementInput { get; private set; }
        public Vector2 AimJoystickInput { get; private set; }

        public int NormInputX { get; private set; }
        public int NormInputY { get; private set; }

        public bool ThrowInput { get; private set; }
        public bool CanThrow { get; set; } = true;
        
        private bool _throwLocked = false;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            
            _controlMode = (ControlMode)SettingsManager.GetControlMode(); // Load the saved mode
            UpdateControlModeUI();
        }

        private void Update()
        {
            if (aimingJoystick && _controlMode == ControlMode.DualJoysticks)
            {
                AimJoystickInput = aimingJoystick.JoystickDirection;
            }
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable(); // Enable Enhanced Touch for better touch detection
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable(); // Disable it when the object is disabled/destroyed
        }
        
        public void SetThrow(InputAction.CallbackContext ctx)
        {
            if (_controlMode == ControlMode.TapToThrow && CanThrow && !_throwLocked) // Prevents multiple throws per tap
            {
                if (ctx.performed)
                {
                    ThrowInput = true;
                    _throwLocked = true; // Lock throwing until reset

                    // Reset ThrowInput AFTER state change
                    StartCoroutine(ResetThrowInput());
                }
            }
        }
        
        public void SetThrowButton()
        {
            if (_controlMode == ControlMode.DualJoysticks && CanThrow && !_throwLocked) // Prevents multiple throws per press
            {
                ThrowInput = true;
                _throwLocked = true;
                StartCoroutine(ResetThrowInput());
            }
        }

        private void UpdateControlModeUI()
        {
            var isJoystickMode = _controlMode == ControlMode.DualJoysticks;
            aimingJoystick.gameObject.SetActive(isJoystickMode);
            throwButton.SetActive(isJoystickMode);
        }
        
        
        public void SetThrowDirection(InputAction.CallbackContext ctx)
        {
            if (_controlMode == ControlMode.TapToThrow)
            {
                RawThrowDirection = ctx.ReadValue<Vector2>();

                if (MainCamera == null) return;

                if (playerInput.currentControlScheme is "Keyboard&Mouse" or "Touch")
                {
                    RawThrowDirection = MainCamera.ScreenToWorldPoint(RawThrowDirection) - transform.position;
                }

                // Keep it as Vector2 instead of Vector2Int for smooth rotation
                ThrowDirectionInput = RawThrowDirection.normalized;
            }
        }
        
        private IEnumerator ResetThrowInput()
        {
            yield return new WaitForEndOfFrame(); // Waits one frame
            ThrowInput = false;
            _throwLocked = false; // Unlock throw after reset
        }
        
        public void SetMove(InputAction.CallbackContext ctx)
        {
            //Reads the Vector2 x,y for movement input
            RawMovementInput = ctx.ReadValue<Vector2>();

            //Normalized X,Y inputs for steady movement
            NormInputX = Mathf.RoundToInt(RawMovementInput.x);
            NormInputY = Mathf.RoundToInt(RawMovementInput.y);
        }
    }

    public enum ControlMode
    {
        TapToThrow,
        DualJoysticks
    }
}

        
    


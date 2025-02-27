using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.InputHandler
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public PlayerInput playerInput;
        
        private static UnityEngine.Camera MainCamera => UnityEngine.Camera.main;
        
        public Vector2 RawThrowDirection { get; private set; }
        public Vector2 ThrowDirectionInput { get; private set; }
        public Vector2 RawMovementInput { get; private set; }

        public int NormInputX { get; private set; }
        public int NormInputY { get; private set; }

        public bool ThrowInput { get; private set; }
        public bool CanThrow { get; set; } = true;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        public void SetMove(InputAction.CallbackContext ctx)
        {
            //Reads the Vector2 x,y for movement input
            RawMovementInput = ctx.ReadValue<Vector2>();

            //Normalized X,Y inputs for steady movement
            NormInputX = Mathf.RoundToInt(RawMovementInput.x);
            NormInputY = Mathf.RoundToInt(RawMovementInput.y);
        }

        public void SetThrow(InputAction.CallbackContext ctx)
        {
            if (CanThrow)
            {
                if (ctx.started)
                {
                    ThrowInput = true;
                }
                if (ctx.canceled)
                {
                    ThrowInput = false;
                }
            }
        }

        public void SetThrowDirection(InputAction.CallbackContext ctx)
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
}

        
    


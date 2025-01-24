using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.InputHandler
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public PlayerInput playerInput;
        
        public Vector2 RawMovementInput { get; private set; }
        
        public int NormInputX { get; private set; }
        public int NormInputY { get; private set; }

        public void SetMove(InputAction.CallbackContext ctx)
        {
            //Reads the Vector2 x,y for movement input
            RawMovementInput = ctx.ReadValue<Vector2>();
            
            //Normalized X,Y inputs for steady movement
            NormInputX = Mathf.RoundToInt(RawMovementInput.x);
            NormInputY = Mathf.RoundToInt(RawMovementInput.y);
        }
    }
}

using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.InputHandler
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerDataSo playerData;
        public PlayerInput playerInput;
        public bool throwInput { get; private set; }

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

        public void SetThrow(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                throwInput = true; 
            }
        }

        private void Update()
        {
           
            if (throwInput)
            {
                ThrowPotion();
                throwInput = false; 
            }
        }

        private void ThrowPotion()
        {
            if (playerData == null || playerData.projectile == null)
            {
                Debug.LogWarning("PlayerDataSO or projectile prefab is missing. Potion cannot be thrown.");
                return;
            }

           
            GameObject potion = Instantiate(playerData.projectile, transform.position, Quaternion.identity);

            
            Rigidbody2D potionRb = potion.GetComponent<Rigidbody2D>();
            if (potionRb != null)
            {
                Vector2 throwDirection = Vector2.right * transform.localScale.x;
                potionRb.AddForce(throwDirection * playerData.playerMovementSpeed, ForceMode2D.Impulse);
            }
        }
    }
}

        
    


using _Scripts.CoreSystem;
using _Scripts.InputHandler;
using _Scripts.Player.Player_States;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;

namespace _Scripts.PlayerComponent
{
    public class Player : MonoBehaviour
    {
        #region PlayerStates
        public PlayerStateMachine StateMachine { get; private set; }
        
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        
        [SerializeField] private PlayerDataSo playerData;  // Player data reference
        
        #endregion
        
        #region Components
        
        public Core Core { get; private set; }
        public CollisionSenses CollisionSenses { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }
    
        #endregion
        
        [Header("Player Settings")]
    
        public Rigidbody2D rb;
    
        [Header("Potion Settings")]
        [SerializeField] private GameObject potionPrefab;
        public Transform throwPoint;
        public float throwForce = 10f;

        private Vector2 movementInput;
        private bool isExploding = false;
        private float explosionDuration = 0.5f; // Duration to keep explosion momentum
        private float explosionTimer = 0f;

        #region Unity Callback Functions
        void Awake()
        {
            // Ensure the Rigidbody2D is assigned
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            
            Core = GetComponentInChildren<Core>();
            CollisionSenses = Core.GetCoreComponent<CollisionSenses>();
            
            StateMachine = new PlayerStateMachine();
            
            IdleState = new PlayerIdleState(this, StateMachine, playerData, "isIdle");
            MoveState = new PlayerMoveState(this, StateMachine, playerData, "isMoving");
        }

        private void Start()
        {
            //StateMachine.Initialize(IdleState);
            InputHandler = GetComponent<PlayerInputHandler>();
            
            StateMachine.Initialize(IdleState);
        }

        void Update()
        {
            Core.LogicUpdate();
            StateMachine.CurrentState.LogicUpdate();

            // Throw potion when pressing the space bar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ThrowPotion();
            }

            // Manage explosion timer
            if (isExploding)
            {
                explosionTimer += Time.deltaTime;
                if (explosionTimer >= explosionDuration)
                {
                    isExploding = false;
                    explosionTimer = 0f;
                }
            }
        }

        private void FixedUpdate()
        {
            StateMachine.CurrentState.PhysicsUpdate();
            
            // Preserve horizontal velocity from explosion
            float currentHorizontalVelocity = rb.velocity.x;

            if (isExploding)
            {
                // Apply player input as additional force, not overriding velocity
                Vector2 inputForce = movementInput * playerData.PlayerMovementSpeed;
                rb.AddForce(inputForce, ForceMode2D.Force);
            }
            else
            {
                // Combine player input with current horizontal velocity
                float targetHorizontalVelocity = movementInput.x * playerData.PlayerMovementSpeed;
                float smoothHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, targetHorizontalVelocity, Time.fixedDeltaTime * 5f);
                rb.velocity = new Vector2(smoothHorizontalVelocity, rb.velocity.y);
            }
        }
        #endregion

        public void ApplyExplosionForce(Vector2 explosionDirection, float explosionForce)
        {
            // Apply the explosion force to the player's Rigidbody2D (push away from explosion)
            rb.AddForce(explosionDirection * explosionForce, ForceMode2D.Impulse);
    
            // Set the explosion flag
            isExploding = true;

            // Debugging applied force and velocity
            Debug.Log($"Applying Explosion Force: {explosionDirection * explosionForce}");
            Debug.Log($"Player Velocity After Explosion: {rb.velocity}");
        
            // Wait for the next physics update to log the result
            Invoke("LogPostExplosionVelocity", Time.fixedDeltaTime);
        }

        void LogPostExplosionVelocity()
        {
            Debug.Log($"Player Velocity After Explosion (Post Physics Update): {rb.velocity}");
            Debug.Log($"Player Position After Explosion: {transform.position}");
        }

        private void ThrowPotion()
        {
            if (potionPrefab != null && throwPoint != null)
            {
                // Instantiate the potion at the throwPoint's position
                GameObject potion = Instantiate(potionPrefab, throwPoint.position, Quaternion.identity);

                // Get the Rigidbody2D of the potion to apply force
                Rigidbody2D potionRb = potion.GetComponent<Rigidbody2D>();

                // Calculate the throw direction
                Vector2 throwDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - throwPoint.position).normalized;

                // Apply force to the potion
                potionRb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
            }
        }

        private void OnDrawGizmos()
        {
            if (Core == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(CollisionSenses.GroundCheck.position, CollisionSenses.GroundCheckRadius) ;
        }
    }
}
using System.Collections;
using _Scripts.CoreSystem;
using _Scripts.InputHandler;
using _Scripts.Player;
using _Scripts.Player.Player_States;
using _Scripts.Player.Player_States.SubStates;
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
        //public PlayerMoveState MoveState { get; private set; }
        public PlayerThrowState ThrowState { get; private set; }
        public PlayerInAirState InAirState { get; private set; }
        
        [SerializeField] private PlayerDataSo playerData;  // Player data reference
        
        #endregion
        
        #region Components
        
        public Core Core { get; private set; }
        public CollisionSenses CollisionSenses { get; private set; }
        public Movement Movement { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }
        public Animator MyAnimator { get; private set; }
        public AnimationEventHandler AnimationEventHandler { get; private set; }
        public Transform ThrowDirectionIndicator { get; private set; }
    
        #endregion
        
        public bool WasThrowingInAir { get; set; }
        
        #region Unity Callback Functions

        private void Awake()
        {
            Core = GetComponentInChildren<Core>();
            CollisionSenses = Core.GetCoreComponent<CollisionSenses>();
            Movement = Core.GetCoreComponent<Movement>();
            
            StateMachine = new PlayerStateMachine();
            
            IdleState = new PlayerIdleState(this, StateMachine, playerData, "isIdle");
            //MoveState = new PlayerMoveState(this, StateMachine, playerData, "isMoving");
            ThrowState = new PlayerThrowState(this, StateMachine, playerData, "isThrowing");
            InAirState = new PlayerInAirState(this, StateMachine, playerData, "isInAir");
        }

        private void Start()
        {
            
            MyAnimator = GetComponent<Animator>();
            InputHandler = GetComponent<PlayerInputHandler>();
            AnimationEventHandler = GetComponent<AnimationEventHandler>();
            
            ThrowDirectionIndicator = transform.Find("ThrowDirectionIndicator");
            
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            Core.LogicUpdate();
            StateMachine.CurrentState.LogicUpdate();
            
            float targetAngle = Vector2.SignedAngle(Vector2.right, InputHandler.ThrowDirectionInput);

            // Smoothly interpolate rotation using Quaternion.Lerp
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 45f);
            ThrowDirectionIndicator.rotation = Quaternion.Lerp(
                ThrowDirectionIndicator.rotation,
                targetRotation,
                Time.deltaTime * playerData.rotationSpeed
            );
        }

        private void FixedUpdate()
        {
            StateMachine.CurrentState.PhysicsUpdate();
        }
        #endregion

        private void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

        private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
        
        // Method to start the smooth fall transition
        public void StartFallTransition(Movement movement, float transitionTime, float targetGravity)
        {
            StartCoroutine(SmoothFall(movement, transitionTime, targetGravity));
        }

        private IEnumerator SmoothFall(Movement movement, float transitionTime, float targetGravity)
        {
            float elapsedTime = 0f;
            float initialGravity = movement.Rb2D.gravityScale;

            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                movement.Rb2D.gravityScale = Mathf.Lerp(initialGravity, targetGravity, elapsedTime / transitionTime);
                yield return null;
            }

            movement.Rb2D.gravityScale = targetGravity;
        }

        private void OnDrawGizmos()
        {
            if (Core == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(CollisionSenses.GroundCheck.position, CollisionSenses.GroundCheckRadius) ;
        }
    }
}
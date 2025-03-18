using System;
using _Scripts.CoreSystem;
using _Scripts.InputHandler;
using _Scripts.Managers.Spawn_Logic;
using _Scripts.Player;
using _Scripts.Player.Player_States;
using _Scripts.Player.Player_States.SubStates;
using _Scripts.PlayerState;
using _Scripts.ScriptableObjects.PlayerData;
using UnityEngine;
using UnityEngine.Serialization;

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
        public Stats Stats { get; private set; }
        public Movement Movement { get; private set; }
        public PlayerInputHandler InputHandler { get; private set; }
        public Animator MyAnimator { get; private set; }
        public Transform ThrowDirectionIndicator { get; private set; }
        
        public SpawnManager SpawnManager { get; private set; }

    
        #endregion
        
        #region Other Variables
        
        private bool _hasBeenHit = false; // Prevent multiple triggers
        public bool WasThrowingInAir { get; set; }
        
        #endregion
        
        #region Unity Callback Functions

        private void Awake()
        {
            Core = GetComponentInChildren<Core>();
            CollisionSenses = Core.GetCoreComponent<CollisionSenses>();
            Movement = Core.GetCoreComponent<Movement>();
            Stats = Core.GetCoreComponent<Stats>();
            
            StateMachine = new PlayerStateMachine();
            
            IdleState = new PlayerIdleState(this, StateMachine, playerData, "isIdle");
            ThrowState = new PlayerThrowState(this, StateMachine, playerData, "isThrowing");
            InAirState = new PlayerInAirState(this, StateMachine, playerData, "isInAir");
        }

        private void Start()
        {
            
            MyAnimator = GetComponent<Animator>();
            InputHandler = GetComponent<PlayerInputHandler>();
            
            ThrowDirectionIndicator = transform.Find("ThrowDirectionIndicator");
            
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            Core.LogicUpdate();
            StateMachine.CurrentState.LogicUpdate();
            
            IndicatorRotation();
            JoyStickIndicatorRotation();
            
        }

        private void FixedUpdate()
        {
            StateMachine.CurrentState.PhysicsUpdate();
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hasBeenHit) return; // Prevents re-triggering

            if (other.CompareTag("Lava"))
            {
                Debug.Log("Hit by lava");
                Stats.Health.CurrentValue = 0;
                _hasBeenHit = true; // Mark as hit
            }
        }
        

        #endregion

        private void IndicatorRotation()
        {
            var targetAngle = Vector2.SignedAngle(Vector2.right, InputHandler.ThrowDirectionInput);

            // Smoothly interpolate rotation using Quaternion.Lerp
            var targetRotation = Quaternion.Euler(0, 0, targetAngle - 45f);
            
            ThrowDirectionIndicator.rotation = Quaternion.Lerp(
                ThrowDirectionIndicator.rotation,
                targetRotation,
                Time.deltaTime * playerData.rotationSpeed
            );
        }

        private void JoyStickIndicatorRotation()
        {
            if (InputHandler.AimJoystickInput.magnitude > 0.1f)
            {
                var targetAngle = Vector2.SignedAngle(Vector2.right, InputHandler.AimJoystickInput);
                var targetRotation = Quaternion.Euler(0, 0, targetAngle - 45f);

                ThrowDirectionIndicator.rotation = Quaternion.Lerp(
                    ThrowDirectionIndicator.rotation,
                    targetRotation,
                    Time.deltaTime * playerData.rotationSpeed
                );
            }
        }
        private void ThrowAnimationTrigger() => StateMachine.CurrentState.ThrowAnimationTrigger();

        private void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
        
        // private void OnDrawGizmos()
        // {
        //     if (Core == null) return;
        //     
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawSphere(CollisionSenses.GroundCheck.position, CollisionSenses.GroundCheckRadius) ;
        // }
    }
}
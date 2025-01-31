using _Scripts.Entities.EntityStateMachine;
using UnityEngine;

namespace _Scripts.Entities.EntitySpecific
{
    public class Entity1 : Entity
    {
        [SerializeField] private GameObject test;
        public E1_IdleState IdleState { get; private set; }
        public E1_MoveState MoveState { get; private set; }
        
        public override void Awake()
        {
            base.Awake();
            
            IdleState = new E1_IdleState(this, StateMachine, entityDataSo, "IsIdle", this);
            MoveState = new E1_MoveState(this, StateMachine, entityDataSo, "IsMoving", this);
        }

        public override void Start()
        {
            base.Start();
            
            StateMachine.Initialize(IdleState);
        }

        public override void Update()
        {
            base.Update();
            
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (CollisionSenses.PlayerDetected)
            {
                Debug.Log("Player is detected");
            }
        }

        // Visualize the raycasts
        public override void OnDrawGizmos()
        {
            if(Core == null) return;
            
            Gizmos.color = CollisionSenses.PlayerDetected ? Color.blue : Color.black;
            Gizmos.DrawWireSphere(CollisionSenses.PlayerDetectedCheck.position, CollisionSenses.PlayerDetectedRadius);
            
            var angleStep = 360f / 36; // Dividing circle into 36 steps (10-degree increments)
            float detectionRadius = CollisionSenses.PlayerDetectedRadius;
            Vector3 origin = CollisionSenses.PlayerDetectedCheck.position;

            for (float angle = 0; angle < 360; angle += angleStep)
            {
                // Calculate the direction of the ray
                float radian = angle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

                // Cast a ray in the direction
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, detectionRadius);

                // Draw the ray
                if (hit.collider != null)
                {
                    // If the ray hits something, draw to the hit point
                    Gizmos.color = ((1 << hit.collider.gameObject.layer) & CollisionSenses.WhatIsPlayer) != 0 ? Color.green : Color.red;
                    Gizmos.DrawLine(origin, hit.point);
                }
                else
                {
                    // If the ray hits nothing, draw the full length
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(origin, origin + (Vector3)direction * detectionRadius);
                }
            }
        }
    }
}

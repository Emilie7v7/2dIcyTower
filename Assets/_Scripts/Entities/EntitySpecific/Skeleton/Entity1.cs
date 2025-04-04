using _Scripts.Audio;
using _Scripts.CoreSystem;
using _Scripts.Entities.EntityStateMachine;
using UnityEngine;

namespace _Scripts.Entities.EntitySpecific.Skeleton
{
    public class Entity1 : Entity
    {
        public E1IdleState IdleState { get; private set; }
        public E1MoveState MoveState { get; private set; }
        public E1PlayerDetectedState PlayerDetectedState { get; private set; }
        public E1LongRangeAttackState LongRangeAttackState { get; private set; }
        
        public SkeletonAudio SkeletonAudio { get; private set; }
        
        public override void Awake()
        {
            base.Awake();
            
            IdleState = new E1IdleState(this, StateMachine, entityDataSo, "isIdle", this);
            MoveState = new E1MoveState(this, StateMachine, entityDataSo, "isMoving", this);
            PlayerDetectedState = new E1PlayerDetectedState(this, StateMachine, entityDataSo, "isPlayerDetected", this);
            LongRangeAttackState = new E1LongRangeAttackState(this, StateMachine, entityDataSo, "isLongRangeAttacking", this);
        }

        public override void Start()
        {
            base.Start();

            SkeletonAudio = GetComponent<SkeletonAudio>();
            
            StateMachine.Initialize(IdleState);
        }

        // Visualize the raycasts
        //  public override void OnDrawGizmos()
        //  {
        //      if (Core == null) return;
        //      
        //      Gizmos.color = Color.yellow;
        //      Gizmos.DrawLine(CollisionSenses.LedgeCheckVertical.position, 
        //              new Vector2(CollisionSenses.LedgeCheckVertical.transform.position.x, CollisionSenses.LedgeCheckVertical.position.y - CollisionSenses.LedgeCheckVerticalDistance));
        //      
        //       var angleStep = CollisionSenses.AngleInDegreesForDetectingPlayer; // Dividing circle into 36 steps (10-degree increments)
        //       var detectionDistance = CollisionSenses.PlayerDetectionDistance;
        //       var detectionRadius = CollisionSenses.PlayerDetectionRadius;
        //       var origin = CollisionSenses.PlayerDetectedCheck.position;
        //       var maxHits = CollisionSenses.MaxHitsRay; // Adjust based on expected number of hits
        //       var results = new RaycastHit2D[maxHits];
        //      
        //      
        //       for (float angle = 0; angle < 360; angle += angleStep)
        //       {
        //           // Calculate the direction of the ray
        //           float radian = angle * Mathf.Deg2Rad;
        //           Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        //      
        //           // Cast a ray in the direction
        //           //RaycastHit2D hit = Physics2D.Raycast(origin, direction, detectionDistance);
        //           
        //           int hitCount = Physics2D.CircleCastNonAlloc(origin, detectionRadius, direction, results, detectionDistance);
        //           
        //           // Draw the main detection area (optional, to see the entire scanning radius)
        //           Gizmos.color = new Color(1f, 1f, 0f, 0.3f); // Semi-transparent yellow
        //           Gizmos.DrawWireSphere(origin, detectionRadius); 
        //      
        //           // Process only valid hits
        //           if (hitCount > 0)
        //           {
        //               for (int i = 0; i < hitCount; i++)
        //               {
        //                   RaycastHit2D hit = results[i];
        //      
        //                   if (hit.collider != null)
        //                   {
        //                       // Check if the hit object belongs to the player layer
        //                       Gizmos.color = ((1 << hit.collider.gameObject.layer) & CollisionSenses.WhatIsPlayer) != 0 ? Color.green : Color.red;
        //                   }
        //      
        //                   // Draw a line to each hit point
        //                   Gizmos.DrawLine(origin, hit.point);
        //                   
        //                   // Draw a circle at the hit point to represent the radius of detection
        //                   Gizmos.DrawWireSphere(hit.point, detectionRadius);
        //               }
        //           }
        //           else
        //           {
        //               // If nothing was hit, draw the full length of the detection distance
        //               Gizmos.color = Color.yellow;
        //               Gizmos.DrawLine(origin, origin + (Vector3)direction * detectionDistance);
        //           }
        //       }
        // }
    }
}

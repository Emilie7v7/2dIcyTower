using _Scripts.Generics;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class CollisionSenses : CoreComponent
    {
        private Movement _movement;
        private RaycastHit2D[] _results;

        protected override void Awake()
        {
            base.Awake();

            _movement = Core.GetCoreComponent<Movement>();
            _results = new RaycastHit2D[MaxHitsRay]; // Pre-allocate array for performance
        }

        #region Transform Checks

        public Transform GroundCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(groundCheck, Core.transform.parent.name);
        
            private set => groundCheck = value;
        }

        public Transform EntityCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(entityCheck, Core.transform.parent.name);

            private set => entityCheck = value;
        }

        public Transform PlayerDetectedCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(playerDetectedCheck, Core.transform.parent.name);
            
            private set => playerDetectedCheck = value;
        }

        public Transform LedgeCheckVertical
        {
            get => GenericNotImplementedError<Transform>.TryGet(ledgeCheckVertical, Core.transform.parent.name);
            
            private set => ledgeCheckVertical = value;
        }
        
        [Header("Transform Checks")]
        
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform entityCheck;
        [SerializeField] private Transform ledgeCheckVertical;
        
        [Header("Specific Settings For Enemies")]
        [SerializeField] private Transform playerDetectedCheck;

        #endregion
        
        #region Parameters
        
        public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
        public float EntityCheckRadius { get => entityCheckRadius; set => entityCheckRadius = value; }
        public float LedgeCheckVerticalDistance { get => ledgeCheckVerticalDistance; set => ledgeCheckVerticalDistance = value; }
        public float PlayerDetectionRadius { get => playerDetectionCheckRadius; set => playerDetectionCheckRadius = value; }
        public float PlayerDetectionDistance { get => playerDetectionDistance; set => playerDetectionDistance = value; }
        public float AngleInDegreesForDetectingPlayer { get => angleInDegreesForDetectingPlayer; set => angleInDegreesForDetectingPlayer = value; }
        public int MaxHitsRay { get => maxHitsRay; set => maxHitsRay = value; }
        
        [Header("Radius Checks")]
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private float entityCheckRadius;
        [SerializeField] private float ledgeCheckVerticalDistance;
        
        [Header("Specific Settings For Enemies")]
        [SerializeField] private float playerDetectionCheckRadius;
        [SerializeField] private float playerDetectionDistance;
        [SerializeField] private float angleInDegreesForDetectingPlayer;
        [SerializeField] private int maxHitsRay;

        #endregion

        #region Layers
        
        public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }
        public LayerMask WhatIsPlayer { get => whatIsPlayer; set => whatIsPlayer = value; }
        public LayerMask WhatIsPlatform { get => whatIsPlatform; set => whatIsPlatform = value; }
        public LayerMask WhatIsEnemy { get => whatIsEnemy; set => whatIsEnemy = value; }
        
        [Header("Layer")]
        [SerializeField] private LayerMask whatIsGround, whatIsPlayer, whatIsPlatform, whatIsEnemy;

        #endregion

        #region Bools

        //Checks whether Player or an Entity is grounded or not
        public bool Ground => Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, whatIsGround);
        
        //Check whether Entity is touching Player
        public bool Player => Physics2D.OverlapCircle(EntityCheck.position, EntityCheckRadius, whatIsPlayer);

        //Check whether Entity is touching Enemy
        public bool Enemy => Physics2D.OverlapCircle(EntityCheck.position, EntityCheckRadius, whatIsEnemy);
        
        //Check whether Entity or Player is on the ledge
        public bool LedgeVertical => Physics2D.Raycast(LedgeCheckVertical.position, Vector2.down, LedgeCheckVerticalDistance, whatIsGround);
        
        // Detects if the player is in line of sight using 360-degree CircleCast.
        public bool IsPlayerInLineOfSight()
        {
            Vector2 origin = PlayerDetectedCheck.position;
            var layerMask = ~LayerMask.GetMask("Ground"); // Ignore ground collisions
            var angleStep = AngleInDegreesForDetectingPlayer;

            for (float angle = 0; angle < 360; angle += angleStep)
            {
                var radian = angle * Mathf.Deg2Rad;
                var direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

                var hitCount = Physics2D.CircleCastNonAlloc(origin, PlayerDetectionRadius, direction, _results, PlayerDetectionDistance, layerMask);

                if (hitCount > 0)
                {
                    for (int i = 0; i < hitCount; i++)
                    {
                        RaycastHit2D hit = _results[i];
                
                        if (hit.collider.CompareTag("Player")) // Ensure we detect the player
                        {
                            //Debug.Log("Player detected at: " + hit.point);
                            Debug.DrawLine(origin, hit.point, Color.green, 0.1f);
                            // Add logic to react to player detection (e.g., alert enemy AI)
                            return true; // Exit after detecting the player
                        }
                    }
                }
            }
            return false; // Player not detected
        }
        
        #endregion
        
    }
}

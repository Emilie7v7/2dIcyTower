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
            _results = new RaycastHit2D[MaxHitsRayForEnemy]; // Pre-allocate array for performance
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
        public int MaxHitsRayForEnemy { get => maxHitsRayForEnemy; set => maxHitsRayForEnemy = value; }
        
        [Header("Radius Checks")]
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private float entityCheckRadius;
        [SerializeField] private float ledgeCheckVerticalDistance;
        
        [Header("Specific Settings For Enemies")]
        [SerializeField] private float playerDetectionCheckRadius;
        [SerializeField] private float playerDetectionDistance;
        [SerializeField] private float angleInDegreesForDetectingPlayer;
        [SerializeField] private int maxHitsRayForEnemy;

        #endregion

        #region Layers
        
        public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }
        public LayerMask WhatIsPlayer { get => whatIsPlayer; set => whatIsPlayer = value; }
        //public LayerMask WhatIsPlatform { get => whatIsPlatform; set => whatIsPlatform = value; }
        public LayerMask WhatIsEnemy { get => whatIsEnemy; set => whatIsEnemy = value; }
        
        [Header("Layer")]
        [SerializeField] private LayerMask whatIsGround, whatIsPlayer, whatIsEnemy;

        #endregion

        #region Bools

        //Checks whether Player or an Entity is grounded or not
        public bool Grounded 
        {
            get
            {
                bool isGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, WhatIsGround);
                return isGrounded;
            }
        }
        //Check whether Entity is touching Player
        public bool Player => Physics2D.OverlapCircle(EntityCheck.position, EntityCheckRadius, WhatIsPlayer);

        //Check whether Entity is touching Enemy
        public bool Enemy => Physics2D.OverlapCircle(EntityCheck.position, EntityCheckRadius, WhatIsEnemy);
        
        //Check whether Entity or Player is on the ledge
        public bool LedgeVertical => Physics2D.Raycast(LedgeCheckVertical.position, Vector2.down, LedgeCheckVerticalDistance, WhatIsGround);
        
        // Detects if the player is in line of sight using 360-degree CircleCast.
        public bool IsPlayerInLineOfSight()
        {
            Vector2 origin = PlayerDetectedCheck.position;
            var layerMask = WhatIsPlayer; // Ignore ground collisions
            var angleStep = AngleInDegreesForDetectingPlayer;

            for (float angle = 0; angle < 360; angle += angleStep)
            {
                var radian = angle * Mathf.Deg2Rad;
                var direction = transform.right * Mathf.Cos(radian) + transform.up * Mathf.Sin(radian);

                var hitCount = Physics2D.CircleCastNonAlloc(origin, PlayerDetectionRadius, direction, _results,
                    PlayerDetectionDistance, layerMask);

                if (hitCount > 0)
                {
                    for (var i = 0; i < hitCount; i++)
                    {
                        var hit = _results[i];
                        
                        if (hit.collider.CompareTag("Player"))
                        {
                            Debug.Log("Player detected at: " + hit.point);
                            Debug.DrawLine(origin, hit.point, Color.green, 0.5f);
                            return true;
                        }
                    }
                }
            }
            Debug.Log("Player not found");
            return false; // Player not detected
        }

        #endregion
        
    }
}

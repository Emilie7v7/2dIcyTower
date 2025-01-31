using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class CollisionSenses : CoreComponent
    {
        private Movement _movement;

        protected override void Awake()
        {
            base.Awake();

            _movement = core.GetCoreComponent<Movement>();
        }

        public Transform GroundCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(groundCheck, core.transform.parent.name);
        
            private set => groundCheck = value;
        }

        public Transform EntityCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(entityCheck, core.transform.parent.name);

            private set => entityCheck = value;
        }

        public Transform PlayerDetectedCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(playerDetectedCheck, core.transform.parent.name);
            
            private set => playerDetectedCheck = value;
        }
        
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform entityCheck;
        [SerializeField] private Transform playerDetectedCheck;
        
        
        public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
        public float EntityCheckRadius { get => entityCheckRadius; set => entityCheckRadius = value; }
        public float PlayerDetectedRadius { get => playerDetectedCheckRadius; set => playerDetectedCheckRadius = value; }

        [SerializeField] private float groundCheckRadius;
        [SerializeField] private float entityCheckRadius;
        [SerializeField] private float playerDetectedCheckRadius;

        
        public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }
        public LayerMask WhatIsPlayer { get => whatIsPlayer; set => whatIsPlayer = value; }
        public LayerMask WhatIsPlatform { get => whatIsPlatform; set => whatIsPlatform = value; }
        public LayerMask WhatIsEnemy { get => whatIsEnemy; set => whatIsEnemy = value; }
        
        [SerializeField] private LayerMask whatIsGround, whatIsPlayer, whatIsPlatform, whatIsEnemy;

        
        //Checks whether Player or an Entity is grounded or not
        public bool Ground => Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, whatIsGround);
        
        //Check whether Entity is touching Player
        public bool Player => Physics2D.OverlapCircle(EntityCheck.position, EntityCheckRadius, whatIsPlayer);

        //Check whether Entity is touching Enemy
        public bool Enemy => Physics2D.OverlapCircle(EntityCheck.position, EntityCheckRadius, whatIsEnemy);
        
        public bool PlayerDetected => Physics2D.OverlapCircle(PlayerDetectedCheck.position, playerDetectedCheckRadius, whatIsPlayer);
        
        
        // Detect player with line of sight
        public bool IsPlayerInLineOfSight()
        {
            float angleStep = 360f / 36; // Dividing circle into 36 steps (10-degree increments)
            float detectionRadius = PlayerDetectedRadius;
            Vector3 origin = PlayerDetectedCheck.position;

            for (float angle = 0; angle < 360; angle += angleStep)
            {
                // Calculate the direction of the ray
                float radian = angle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

                // Cast a ray in the direction
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, detectionRadius);

                if (hit.collider != null)
                {
                    // Check if the ray hit the player
                    if (((1 << hit.collider.gameObject.layer) & whatIsPlayer) != 0)
                    {
                        // Ensure the line of sight is not blocked
                        if (!Physics2D.Raycast(origin, direction, hit.distance, whatIsGround))
                        {
                            return true; // Player detected with clear line of sight
                        }
                    }
                }
            }

            return false; // Player not detected
        }
    }
    
}

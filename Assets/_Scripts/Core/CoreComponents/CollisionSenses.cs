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
        
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform entityCheck;
        
        
        public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
        public float EntityCheckRadius { get => entityCheckRadius; set => entityCheckRadius = value; }

        [SerializeField] private float groundCheckRadius;
        [SerializeField] private float entityCheckRadius;

        
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
        }
    
}

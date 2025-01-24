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
        [SerializeField] private Transform groundCheck;
        
        
        public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
        [SerializeField] private float groundCheckRadius;

        
        public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }
        public LayerMask WhatIsPlayer { get => whatIsPlayer; set => whatIsPlayer = value; }
        public LayerMask WhatIsPlatform { get => whatIsPlatform; set => whatIsPlatform = value; }
        [SerializeField] private LayerMask whatIsGround, whatIsPlayer, whatIsPlatform;
        
        
        //Checks whether Player or an Entity is grounded or not
        public bool Ground => Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, whatIsGround);
    }
    
}

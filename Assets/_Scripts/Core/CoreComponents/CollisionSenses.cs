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
       
        public Transform PlayerCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(playerCheck, core.transform.parent.name);
    
            private set => playerCheck = value;
        }
        [SerializeField] private Transform playerCheck;

        public Transform EnemyCheck
        {
            get => GenericNotImplementedError<Transform>.TryGet(enemyCheck, core.transform.parent.name);

            private set => enemyCheck = value;
        }
        [SerializeField] private Transform enemyCheck;
        
        public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
        [SerializeField] private float groundCheckRadius;
        
        public float PlayerCheckRadius { get => playerCheckRadius; set => playerCheckRadius = value; }
        [SerializeField] private float playerCheckRadius;

        public float EnemyCheckRadius { get => enemyCheckRadius; set => enemyCheckRadius = value; }
        [SerializeField] private float enemyCheckRadius;

        
        public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }
        public LayerMask WhatIsPlayer { get => whatIsPlayer; set => whatIsPlayer = value; }
        public LayerMask WhatIsPlatform { get => whatIsPlatform; set => whatIsPlatform = value; }
        public LayerMask WhatIsEnemy { get => whatIsEnemy; set => whatIsEnemy = value; }
        [SerializeField] private LayerMask whatIsGround, whatIsPlayer, whatIsPlatform, whatIsEnemy;
        
        
        //Checks whether Player or an Entity is grounded or not
        public bool Ground => Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, whatIsGround);
        
        //Check whether Entity is touching Player
        public bool Player => Physics2D.OverlapCircle(PlayerCheck.position, playerCheckRadius, whatIsPlayer);

        //Check whether Entity is touching Enemy
        public bool Enemy => Physics2D.OverlapCircle(EnemyCheck.position, enemyCheckRadius, whatIsEnemy);
        }
    
}

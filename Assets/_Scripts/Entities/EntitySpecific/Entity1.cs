using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.EntityData;

namespace _Scripts.Entities.EntitySpecific
{
    public class Entity1 : Entity
    {
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
            
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
        }
    }
}

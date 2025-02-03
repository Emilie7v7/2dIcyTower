using System;
using _Scripts.CoreSystem;
using _Scripts.ScriptableObjects.EntityData;
using UnityEngine;

namespace _Scripts.Entities.EntityStateMachine
{
    public class Entity : MonoBehaviour
    {
        
        [SerializeField] protected EntityDataSo entityDataSo;
        
        protected EntityStateMachine StateMachine {get; private set;}
        
        public Core Core { get; private set; }
        public Animator MyAnimator { get; private set; }
        public AnimationToStateMachine AnimationToStateMachine { get; private set; }
        
        public CollisionSenses CollisionSenses { get; private set; }

        public virtual void Awake()
        {
            Core = GetComponentInChildren<Core>();
            CollisionSenses = Core.GetCoreComponent<CollisionSenses>();
            StateMachine = new EntityStateMachine();
        }

        public virtual void Start()
        {
            MyAnimator = GetComponent<Animator>();
            AnimationToStateMachine = GetComponent<AnimationToStateMachine>();
        }

        public virtual void Update()
        {
            Core.LogicUpdate();
            StateMachine.CurrentState.LogicUpdate();
        }

        public virtual void FixedUpdate()
        {
            StateMachine.CurrentState.PhysicsUpdate();
        }

        public virtual void OnDrawGizmos()
        {
            if (Core != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(CollisionSenses.LedgeCheckVertical.position, 
                    new Vector2(CollisionSenses.LedgeCheckVertical.transform.position.x, CollisionSenses.LedgeCheckVertical.position.y - CollisionSenses.LedgeCheckVerticalDistance));
            }

        }
    }
}

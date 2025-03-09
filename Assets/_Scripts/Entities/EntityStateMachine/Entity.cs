using System;
using _Scripts.CoreSystem;
using _Scripts.Intermediaries;
using _Scripts.Managers.Score_Logic;
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

        private CollisionSenses CollisionSenses { get; set; }
        public Stats Stats { get; set; }

        public virtual void Awake()
        {
            Core = GetComponentInChildren<Core>();
            CollisionSenses = Core.GetCoreComponent<CollisionSenses>();
            Stats = Core.GetCoreComponent<Stats>();
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ScoreManager.Instance.RegisterKill();
                Stats.Health.CurrentValue = 0;
            }
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

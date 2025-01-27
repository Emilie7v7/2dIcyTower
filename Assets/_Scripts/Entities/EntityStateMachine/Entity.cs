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

        public virtual void Awake()
        {
            Core = GetComponentInChildren<Core>();
            StateMachine = new EntityStateMachine();
        }

        public virtual void Start()
        {
            
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
            if(Core == null) return;
        }
    }
}

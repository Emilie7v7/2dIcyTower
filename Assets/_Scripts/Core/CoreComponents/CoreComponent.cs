using _Scripts.Interfaces;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class CoreComponent : MonoBehaviour, ILogicUpdate
    {
        protected Core core;

        protected virtual void Awake()
        {
            core = transform.parent.GetComponent<Core>();
        
            if (core == null) { Debug.LogWarning("There isn't Core on the parent"); }

            core.AddComponent(this);
        }

        public virtual void LogicUpdate() { }
    }
}

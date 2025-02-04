using _Scripts.Interfaces;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class CoreComponent : MonoBehaviour, ILogicUpdate
    {
        protected Core Core;

        protected virtual void Awake()
        {
            Core = transform.parent.GetComponent<Core>();
        
            if (Core == null) { Debug.LogWarning("There isn't Core on the parent"); }

            Core.AddComponent(this);
        }

        public virtual void LogicUpdate() { }
    }
}


using UnityEngine;

namespace _Scripts.CoreSystem
{
    public abstract class CoreComp<T> where T : CoreComponent
    {
        private readonly Core _core;
        private T _component;

        public T Component => _component ? _component : _core.GetCoreComponent(ref _component);

        public CoreComp(Core core)
        {
            if (core == null)
            {
                Debug.LogWarning($"Core is Null for component {typeof(T)}");
            }
            
            _core = core;
        }
    }
}

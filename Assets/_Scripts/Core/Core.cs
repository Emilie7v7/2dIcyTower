using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class Core : MonoBehaviour
    {
        [field: SerializeField] public GameObject Root { get; private set; }
    
        private readonly List<CoreComponent> coreComponents = new List<CoreComponent>();

        private void Awake()
        {
            Root = Root ? Root : transform.parent.gameObject;
        }

        public void LogicUpdate()
        {
            foreach (var component in coreComponents)
            {
                component.LogicUpdate();
            }
        }

        public void AddComponent(CoreComponent component)
        {
            if (!coreComponents.Contains(component))
            {
                coreComponents.Add(component);
            }
        }
    
        //Null check in case the GameObject is trying to use a specific component.
        //In case the GameObject does not have the required component
        //it will throw a LogWarning check to warn us that it's trying to access something that it doesn't have yet

        public T GetCoreComponent<T>() where T : CoreComponent
        {
            var component = coreComponents.OfType<T>().FirstOrDefault();

            if (component) return component;

            component = GetComponentInChildren<T>();
        
            if (component) return component;
        
            Debug.LogWarning($"CoreComponent {typeof(T)} not found in {Root.name}");
            return null;
        }

        public T GetCoreComponent<T>(ref T value) where T : CoreComponent
        {
            value = GetCoreComponent<T>();
            return value;
        }
    }
}

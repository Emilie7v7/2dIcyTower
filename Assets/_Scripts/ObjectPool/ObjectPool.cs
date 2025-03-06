using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.ObjectPool
{
    public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected int poolSize = 20;

        protected readonly Queue<T> Pool = new Queue<T>();
        protected Transform PoolParent;

        protected abstract void Awake();
        protected abstract void InitializePool();

        public T GetObject(Vector3Int position)
        {
            while (Pool.Count > 0)
            {
                var obj = Pool.Dequeue();

                if (obj is null) //Check if the object was destroyed
                {
                    continue; //Skip destroyed objects
                }

                obj.transform.position = position;
                obj.gameObject.SetActive(true);
                return obj;
            }
            return null;
        }

        public void ReturnObject(T obj)
        {
            if (obj is null)
            {
                return;
            }
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(PoolParent);
            Pool.Enqueue(obj);
        }
    }
}

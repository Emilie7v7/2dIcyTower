using System.Collections.Generic;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.ObjectPool
{
    public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected int poolSize = 20;

        protected readonly Queue<T> Pool = new Queue<T>();
        protected Transform PoolParent;

        protected abstract void Awake();
        protected abstract void InitializePool();

        public T GetObject(Vector3 position)
        {
            while (Pool.Count > 0)
            {
                var obj = Pool.Dequeue();

                switch (obj)
                {
                    //Check if the object was destroyed
                    case null:
                        continue; //Skip destroyed objects
                    //If it's a projectile, update its spawn position
                    case Projectile projectile:
                        projectile.SetSpawnPosition(position);
                        break;
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
            obj.transform.position = Vector3.zero;
            Pool.Enqueue(obj);
        }
    }
}

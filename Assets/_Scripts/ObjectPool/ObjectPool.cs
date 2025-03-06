using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.ObjectPool
{
    public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected T[] prefabs;
        [SerializeField] protected int poolSize = 20;

        private readonly Queue<T> _pool = new Queue<T>();
        private Transform _poolParent;

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            if (prefabs.Length == 0) return;
            
            var perPrefabCount = Mathf.Max(1, poolSize / prefabs.Length);

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null)
            {
                parentObj = new GameObject("PooledObjects");
            }
            _poolParent = parentObj.transform;

            foreach (var prefab in prefabs)
            {
                for (var i = 0; i < perPrefabCount; i++)
                {
                    var obj = Instantiate(prefab, _poolParent);
                    obj.gameObject.SetActive(false);
                    _pool.Enqueue(obj);
                }
            }
        }

        public T GetObject(Vector3Int position)
        {
            if (_pool.Count > 0)
            {
                var obj = _pool.Dequeue();
                obj.transform.position = position;
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                Debug.LogWarning("Pool Empty! Expanding...");
                return Instantiate(GetRandomPrefab(), position, Quaternion.identity);
            }
        }

        public void ReturnObject(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_poolParent);
            _pool.Enqueue(obj);
        }
        
        private T GetRandomPrefab()
        {
            return prefabs[Random.Range(0, prefabs.Length)];
        }
    }
}

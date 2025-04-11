using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class ArrowPool : ObjectPool<Arrow>
    {
        
        [SerializeField] private Arrow arrowPrefab;
        public static ArrowPool Instance { get; private set; }

        protected override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePool();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected override void InitializePool()
        {
            if (arrowPrefab == null) return;

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null)
            {
                parentObj = new GameObject("PooledObjects");
            }

            PoolParent = parentObj.transform;
            DontDestroyOnLoad(parentObj);

            for (var i = 0; i < poolSize; i++)
            {
                var arrow = Instantiate(arrowPrefab, PoolParent);
                arrow.gameObject.SetActive(false);
                Pool.Enqueue(arrow);
            }
        }
    }
}

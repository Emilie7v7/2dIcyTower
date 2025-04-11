using System;
using _Scripts.Objects;
using UnityEngine;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class WoodenChestPool : ObjectPool<Chest>
    {
        [SerializeField] private Chest woodenChestPrefab;
        public static WoodenChestPool Instance { get; private set; }

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
            if (woodenChestPrefab == null) return;

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null)
            {
                parentObj = new GameObject("PooledObjects");
            }

            PoolParent = parentObj.transform;
            DontDestroyOnLoad(parentObj);

            for (var i = 0; i < poolSize; i++)
            {
                var woodChest = Instantiate(woodenChestPrefab, PoolParent);
                woodChest.gameObject.SetActive(false);
                Pool.Enqueue(woodChest);
            }
        }
    }
}

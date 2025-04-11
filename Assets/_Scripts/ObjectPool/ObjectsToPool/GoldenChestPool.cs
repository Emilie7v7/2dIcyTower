using System;
using _Scripts.Objects;
using UnityEngine;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class GoldenChestPool : ObjectPool<Chest>
    {
        [SerializeField] private Chest goldenChestPrefab;
        public static GoldenChestPool Instance { get; private set; }

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
            if (goldenChestPrefab == null) return;

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null)
            {
                parentObj = new GameObject("PooledObjects");
            }

            PoolParent = parentObj.transform;
            DontDestroyOnLoad(parentObj);

            for (var i = 0; i < poolSize; i++)
            {
                var goldChest = Instantiate(goldenChestPrefab, PoolParent);
                goldChest.gameObject.SetActive(false);
                Pool.Enqueue(goldChest);
            }
        }
    }
}

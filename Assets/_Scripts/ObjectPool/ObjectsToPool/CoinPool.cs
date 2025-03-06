using _Scripts.Pickups;
using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEngine;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class CoinPool : ObjectPool<CoinPickup>
    {
        [SerializeField] private ObjectSpawnSettingsSo settings;
        public static CoinPool Instance { get; private set; }

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
            if (settings == null || settings.coinsPrefab == null) return;

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null) 
            {
                parentObj = new GameObject("PooledObjects");
            }
            PoolParent = parentObj.transform;
            DontDestroyOnLoad(parentObj);

            for (var i = 0; i < poolSize; i++)
            {
                var coin = Instantiate(settings.coinsPrefab.GetComponent<CoinPickup>(), PoolParent);
                coin.gameObject.SetActive(false);
                Pool.Enqueue(coin);
            }
        }
    }
}

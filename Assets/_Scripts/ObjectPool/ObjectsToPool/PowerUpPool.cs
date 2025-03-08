using _Scripts.Pickups;
using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEngine;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class PowerUpPool : ObjectPool<PowerUp>
    {
        [SerializeField] private ObjectSpawnSettingsSo settings;
        
        public static PowerUpPool Instance { get; private set; }

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
            if (settings == null || settings.boosts.Length == 0) return;

            var perPowerUpCount = Mathf.Max(1, poolSize / settings.boosts.Length);

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null) 
            {
                parentObj = new GameObject("PooledObjects");
            }
            PoolParent = parentObj.transform;
            DontDestroyOnLoad(parentObj);
            
            foreach (var boost in settings.boosts)
            {
                if (boost.boostPrefab == null) continue; //Safety check

                for (var i = 0; i < perPowerUpCount; i++)
                {
                    var powerUp = Instantiate(boost.boostPrefab.GetComponent<PowerUp>(),PoolParent);
                    powerUp.gameObject.SetActive(false);
                    Pool.Enqueue(powerUp);
                }
            }
        }
        
        public PowerUp GetSpecificPowerUp(GameObject requestedPrefab, Vector3 position)
        {
            foreach (var powerUp in Pool)
            {
                if (powerUp.gameObject.name.Contains(requestedPrefab.name))
                {
                    Pool.Dequeue(); //Remove from pool
                    powerUp.transform.position = position;
                    powerUp.gameObject.SetActive(true);
                    return powerUp;
                }
            }

            Debug.LogWarning($"⚠ No available {requestedPrefab.name} in the pool!");
            return null; // No power-up found
        }
        
    }
}

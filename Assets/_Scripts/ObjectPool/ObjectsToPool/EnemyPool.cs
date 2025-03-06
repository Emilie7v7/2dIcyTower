using _Scripts.Entities.EntitySpecific;
using _Scripts.Entities.EntityStateMachine;
using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEngine;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class EnemyPool : ObjectPool<Entity>
    {
        [SerializeField] private ObjectSpawnSettingsSo settings;
        public static EnemyPool Instance { get; private set; }
        
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
            if (settings == null || settings.enemiesPrefab == null) return;

            var perEnemyCount = Mathf.Max(1, poolSize / settings.enemiesPrefab.Length);

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null)
            {
                parentObj = new GameObject("PooledObjects");
            }
            PoolParent = parentObj.transform;
            DontDestroyOnLoad(parentObj);

            foreach (var enemyPrefab in settings.enemiesPrefab)
            {
                for (var i = 0; i < perEnemyCount; i++)
                {
                    var enemy = Instantiate(enemyPrefab, PoolParent).GetComponent<Entity>();

                    if (enemy == null)
                    {
                        continue;
                    }
                    enemy.gameObject.SetActive(false);
                    Pool.Enqueue(enemy);
                }
            }
        }
    }
}

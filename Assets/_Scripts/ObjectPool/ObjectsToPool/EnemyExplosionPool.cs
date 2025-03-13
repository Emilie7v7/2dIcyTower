using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class EnemyExplosionPool : ObjectPool<Explosion>
    {
        [SerializeField] private Explosion enemyExplosion;
        
        public static EnemyExplosionPool Instance { get; private set; }

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
            if (enemyExplosion is null) return;

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null)
            {
                parentObj = new GameObject("PooledObjects");
            }
            PoolParent = parentObj.transform;
            DontDestroyOnLoad(gameObject);

            for (var i = 0; i < poolSize; i++)
            {
                var newExplosion = Instantiate(enemyExplosion, PoolParent);
                newExplosion.gameObject.SetActive(false);
                Pool.Enqueue(newExplosion);
            }
        }
    }
}

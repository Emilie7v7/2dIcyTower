using _Scripts.Projectiles;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class EnemyProjectilePool : ObjectPool<Projectile>
    {
        [SerializeField] private Projectile enemyProjectilePrefab;
        public static EnemyProjectilePool Instance { get; private set; }

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
            if (enemyProjectilePrefab == null) return;

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null)
            {
                parentObj = new GameObject("PooledObjects");
            }
            PoolParent = parentObj.transform;
            DontDestroyOnLoad(parentObj);

            for (var i = 0; i < poolSize; i++)
            {
                var projectile = Instantiate(enemyProjectilePrefab, PoolParent);
                projectile.gameObject.SetActive(false);
                Pool.Enqueue(projectile);
            }
        }
    }
}
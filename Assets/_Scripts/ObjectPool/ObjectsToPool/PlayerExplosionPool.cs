using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class PlayerExplosionPool : ObjectPool<Explosion>
    {
        [SerializeField] private Explosion playerExplosion;
        
        public static PlayerExplosionPool Instance { get; private set; }


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
            if(playerExplosion is null) return;

            var parentObj = GameObject.Find("PooledObjects");
            if (parentObj == null)
            {
                parentObj = new GameObject("PooledObjects");
            }
            PoolParent = parentObj.transform;
            
            DontDestroyOnLoad(parentObj);

            for (var i = 0; i < poolSize; i++)
            {
                var newPlayerExplosion= Instantiate(playerExplosion, PoolParent);
                newPlayerExplosion.gameObject.SetActive(false);
                Pool.Enqueue(newPlayerExplosion);
            }
        }
    }
}

using _Scripts.Pickups;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class CoinPool : ObjectPool<CoinPickup>
    {
        public static CoinPool Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}

using _Scripts.Pickups;

namespace _Scripts.ObjectPool.ObjectsToPool
{
    public class PowerUpPool : ObjectPool<PowerUp>
    {
        public static PowerUpPool Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

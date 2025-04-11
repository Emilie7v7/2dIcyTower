using _Scripts.ObjectPool.ObjectsToPool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Objects
{
    public class Chest : MonoBehaviour
    {
        private enum ChestType {ChooseType ,Golden, Wooden }

        [Header("General Settings")]
        [SerializeField] private ChestType chestType = ChestType.ChooseType;
        [SerializeField] private GameObject openedChest;

        [Header("Burst Settings")]
        [SerializeField] private float burstStrengthX = 3f;
        [SerializeField] private float burstStrengthYMin = 3f;
        [SerializeField] private float burstStrengthYMax = 5f;

        [Header("Golden Settings")] 
        [SerializeField] private int minGoldCoins;
        [SerializeField] private int maxGoldCoins;
        
        [Header("Wooden Settings")]
        [SerializeField] private int minWoodCoins;
        [SerializeField] private int maxWoodCoins;
        
        private static readonly int Exploded = Animator.StringToHash("exploded");
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            openedChest.SetActive(false);
        }

        public void OpenChest()
        {
            Debug.Log("Chest got hit");
            _animator.SetBool(Exploded, true);
            openedChest.SetActive(true);
            switch (chestType)
            {
                case ChestType.Golden:
                {
                    var amountOfCoins = Random.Range(minGoldCoins, maxGoldCoins + 1);

                    for (var i = 0; i < amountOfCoins; i++)
                    {
                        var coin = ChestCoinPool.Instance.GetObject(transform.position);
                        var rb = coin.GetComponent<Rigidbody2D>();
                        
                        if (rb)
                        {
                            var burstX = Random.Range(-burstStrengthX, burstStrengthX);
                            var burstY = Random.Range(burstStrengthYMin, burstStrengthYMax);
                            rb.velocity = Vector2.zero;
                            rb.gravityScale = 2f;
                            rb.AddForce(new Vector2(burstX, burstY), ForceMode2D.Impulse);
                        }
                    }

                    break;
                }
                case ChestType.Wooden:
                {
                    var amountOfCoins = Random.Range(minWoodCoins, maxWoodCoins + 1);

                    for (var i = 0; i < amountOfCoins; i++)
                    {
                        var coin = ChestCoinPool.Instance.GetObject(transform.position);
                        var rb = coin.GetComponent<Rigidbody2D>();
                        
                        if (rb)
                        {
                            var burstX = Random.Range(-burstStrengthX, burstStrengthX);
                            var burstY = Random.Range(burstStrengthYMin, burstStrengthYMax);
                            rb.velocity = Vector2.zero;
                            rb.gravityScale = 2f;
                            rb.AddForce(new Vector2(burstX, burstY), ForceMode2D.Impulse);
                        }
                    }

                    break;
                }
            }
        }

        public void ResetChest()
        {
            _animator.SetBool(Exploded, false);
            openedChest.SetActive(false);
        }
    }
}

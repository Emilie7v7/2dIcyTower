using _Scripts.Managers.GameManager;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class CoinPickup : MonoBehaviour
    {
        [SerializeField] private int coinValue = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.AddCoins(coinValue);
                Destroy(gameObject);
            }
        }
    }
}

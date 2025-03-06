using System;
using _Scripts.CoreSystem;
using _Scripts.Managers.GameManager;
using _Scripts.ObjectPool.ObjectsToPool;
using UnityEngine;

namespace _Scripts.Pickups
{
    public class ImmortalityPickup : PowerUp
    {
        private float _immortalityDuration;

        private void Start()
        {
            _immortalityDuration = GameManager.Instance.PlayerData.immortalityDuration;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var stats = other.GetComponentInChildren<Stats>();
                if (stats != null)
                {
                    stats.ActivateImmortality(_immortalityDuration);
                }

                PowerUpPool.Instance.ReturnObject(this);
            }
        }
    }
}

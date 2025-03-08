using System;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.ScriptableObjects.SpawnSettingsData;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Managers.Drop_Items_Logic
{
    public class DropManager : MonoBehaviour
    {
        public static DropManager Instance { get; private set; }
        
        [SerializeField] private ObjectSpawnSettingsSo settings;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private GameObject GetDroppedItem()
        {
            var totalWeight = 0f;

            foreach (var boosts in settings.boosts)
            {
                totalWeight += boosts.dropChance;
                var stringName = $"{boosts.boostName}: {boosts.dropChance}";
                Debug.Log(stringName);
            }
            
            var randomValue = Random.Range(0, totalWeight);
            var cumulativeWeight = 0f;
            
            foreach (var boosts in settings.boosts)
            {
                cumulativeWeight += boosts.dropChance;
                if (randomValue <= cumulativeWeight)
                {
                    //Debug.Log($"Random value is: {randomValue} \n Dropping this boost: {boosts.boostName}");
                    return boosts.boostPrefab;
                }
            }
            Debug.LogWarning($"⚠ No boost was selected! (Random: {randomValue}, TotalWeight: {totalWeight})");
            return null;
        }

        public void SpawnDroppedItem(Vector3 dropPosition)
        {
            var dropChance = settings.dropChanceEnemy / 100f;
            
            if (Random.value <= dropChance)
            {
                var boostPrefab = GetDroppedItem();
                if (boostPrefab is not null)
                {
                    var boost = PowerUpPool.Instance.GetSpecificPowerUp(boostPrefab, dropPosition);

                    if (boost is not null)
                    {
                        boost.transform.position = dropPosition;
                        boost.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogWarning($"⚠ PowerUp Pool Empty! Could not drop {boostPrefab.name}");
                    }
                }
            }
        }
    }
}

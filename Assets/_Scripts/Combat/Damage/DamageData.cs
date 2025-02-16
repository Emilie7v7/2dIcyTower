using UnityEngine;

namespace _Scripts.Combat.Damage
{
    public class DamageData
    {
        public int Amount { get; private set; }
        public GameObject Source { get; private set; }

        public DamageData(int amount, GameObject source)
        {
            Amount = amount;
            Source = source;
        }
        
        public void SetAmount(int amount) => Amount = amount;
    }
}

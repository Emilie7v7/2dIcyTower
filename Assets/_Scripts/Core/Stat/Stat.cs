using System;
using UnityEngine;

namespace _Scripts.CoreSystem.StatSystem

{
    [Serializable]
    public class Stat
    {
        public event Action OnCurrentValueZero;
        
        [field: SerializeField] public float MaxValue { get; private set; }

        public float CurrentValue
        {
            get => _currentValue;

            set
            {
                _currentValue = Mathf.Clamp(value, 0, MaxValue);
                if (_currentValue <= 0)
                {
                    OnCurrentValueZero?.Invoke();
                }
            }
        }
        private float _currentValue;
        
        public void Initialize() => CurrentValue = MaxValue;
        
        public void IncreaseAmount(float amount) => CurrentValue += amount;
        public void DecreaseAmount(float amount) => CurrentValue -= amount;
    }
}

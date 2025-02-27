using System;
using UnityEngine;

namespace _Scripts.CoreSystem.StatSystem

{
    [Serializable]
    public class Stat
    {
        public event Action OnCurrentValueZero;
        public event Action OnValueChanged;
        
        [field: SerializeField] public int MaxValue { get; set; }


        public int CurrentValue
        {
            get => _currentValue;

            set
            {
                _currentValue = Mathf.Clamp(value, 0, MaxValue);
                OnValueChanged?.Invoke();
                
                if (_currentValue <= 0)
                {
                    OnCurrentValueZero?.Invoke();
                }
            }
        }
        private int _currentValue;
        
        public void Initialize() => CurrentValue = MaxValue;
        
        public void IncreaseAmount(int amount) => CurrentValue += amount;
        public void DecreaseAmount(int amount) => CurrentValue -= amount;
    }
}

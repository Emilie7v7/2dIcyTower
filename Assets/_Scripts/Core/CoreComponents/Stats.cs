using System;
using _Scripts.CoreSystem.StatSystem;
using _Scripts.Managers.GameManager;
using _Scripts.Managers.UI;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class Stats : CoreComponent
    {
        [field: SerializeField] public Stat Health { get; set; }
        private HealthUIManager _healthUIManager;
        private bool _isPlayer;

        protected override void Awake()
        {
            base.Awake();
            
            Health.Initialize();
            
            _isPlayer = gameObject.CompareTag("Player");

            if (_isPlayer)
            {
                Health.MaxValue = GameManager.Instance.PlayerData.maxHealth;
                Health.Initialize();

                _healthUIManager = FindObjectOfType<HealthUIManager>();

                if (_healthUIManager != null)
                {
                    _healthUIManager.InitializeHealthUI(Health.MaxValue, Health.CurrentValue);
                }
                
                //Only subscribe the UI update for Player
                Health.OnValueChanged += UpdateHealthUI;
            }
        }

        private void UpdateHealthUI()
        {
            if (_healthUIManager != null && _isPlayer)
            {
                _healthUIManager.UpdateHealth(Health.CurrentValue);
            }
        }

        private void OnDestroy()
        {
            Health.OnValueChanged -= UpdateHealthUI;
        }
    }
}

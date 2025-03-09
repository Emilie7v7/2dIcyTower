using System.Collections;
using _Scripts.CoreSystem.StatSystem;
using _Scripts.Managers.Game_Manager_Logic;
using _Scripts.Managers.Score_Logic;
using _Scripts.Managers.UI_Logic;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class Stats : CoreComponent
    {
        [field: SerializeField] public Stat Health { get; set; }
        private HealthUIManager _healthUIManager;
        private bool _isPlayer;

        public bool IsImmortal { get; private set; } = false;

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
                Health.OnCurrentValueZero += SaveDataUponDeath;
            }
            else
            {
                Health.OnCurrentValueZero += HandleEnemyDeath;
            }
        }


        private void HandleEnemyDeath()
        {   
            ScoreManager.Instance.RegisterKill();
        }
        
        public void ActivateImmortality(float duration)
        {
            StartCoroutine(ImmortalityCoroutine(duration));
        }
        
        private IEnumerator ImmortalityCoroutine(float duration)
        {
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                IsImmortal = true;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            IsImmortal = false;
        }
        
        private static void SaveDataUponDeath()
        {
            GameManager.Instance.SaveGameData();
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
            Health.OnCurrentValueZero -= SaveDataUponDeath;
            Health.OnCurrentValueZero -= HandleEnemyDeath;
        }
    }
}

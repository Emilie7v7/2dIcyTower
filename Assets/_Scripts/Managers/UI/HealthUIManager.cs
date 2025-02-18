using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers.UI
{
    public class HealthUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject healthPrefab;
        [SerializeField] private Transform healthContainer;
        private readonly List<GameObject> _hearts = new();

        private int _maxHealth;
        private int _currentHealth;

        public void InitializeHealthUI(int startingMaxHealth, int startingHealth)
        {
            _maxHealth = startingMaxHealth; // 🔹 Correctly assign max health
            _currentHealth = startingHealth; // 🔹 Correctly assign current health

            RefreshUI();
        }

        public void UpdateHealth(int newHealth)
        {
            _currentHealth = Mathf.Clamp(newHealth, 0, _maxHealth);
            RefreshUI();
        }

        private void RefreshUI()
        {
            var heartCount = _hearts.Count;
            
            //Add missing hearts if max health increased
            for (var i = heartCount; i < _maxHealth; i++)
            {
                var newHeart = Instantiate(healthPrefab, healthContainer);
                _hearts.Add(newHeart);
            }
            
            //Hide extra hearts if max health decreased
            for (var i = _maxHealth; i < heartCount; i++)
            {
                _hearts[i].SetActive(false);
            }
            
            //Update heart visibility based on current health
            for (var i = 0; i < _maxHealth; i++)
            {
                _hearts[i].GetComponent<Image>().enabled = (i < _currentHealth);
            }
        }
    }
}

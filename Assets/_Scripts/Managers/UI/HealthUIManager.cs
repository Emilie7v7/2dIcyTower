using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Managers.UI
{
    public class HealthUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject healthPrefab;
        [SerializeField] private Transform healthContainer;
        private List<GameObject> _hearts = new();

        private int _maxHealth;
        private int _currentHealth;

        public void InitializeHealthUI(int startingMaxHealth, int startingHealth)
        {
            _maxHealth = GameManager.GameManager.Instance.MaxHealth;
            _currentHealth = startingHealth;
            UpdateHearts();
        }

        public void UpdateHealth(int newHealth)
        {
            _currentHealth = Mathf.Clamp(newHealth, 0, _maxHealth);
            UpdateHearts();
        }

        private void UpdateHearts()
        {
            //Remove excess hearts if reducing max health
            while (_hearts.Count > _maxHealth)
            {
                Destroy(_hearts[^1]);
                _hearts.RemoveAt(_hearts.Count - 1);
            }
            
            //Add missing hearts if increasing max health
            while (_hearts.Count < _maxHealth)
            {
                var newHeart = Instantiate(healthPrefab, healthContainer);
                _hearts.Add(newHeart);
            }
            
            //Update heart visuals based on current health
            for (var i = 0; i < _hearts.Count; i++)
            {
                _hearts[i].GetComponent<Image>().enabled = (i < _currentHealth);
            }
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider; 
    [SerializeField] private int _maxHealth = 100;

    private int currentHealth; 

    private void Start()
    {
        InitializeHealthUI();
        SetCurrentHealth(_maxHealth);
    }
    

    private void InitializeHealthUI()
    {
        // Ensure the slider is properly set up
        if (_healthSlider != null)
        {
            _healthSlider.minValue = 0;
            _healthSlider.maxValue = _maxHealth; 
            _healthSlider.value = _maxHealth; 
        }
        else
        {
            Debug.LogError("Health Slider is not assigned!");
        }
    }

    public void SetCurrentHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, _maxHealth);
        UpdateHealthUI();
    }

    public void ModifyHealth(int amount)
    {
        SetCurrentHealth(currentHealth + amount);
    }

    private void UpdateHealthUI()
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = currentHealth; 
        }
        else
        {
            Debug.LogError("Health Slider is no assigned!");
            //test
        }
    }
}
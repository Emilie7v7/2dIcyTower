using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [FormerlySerializedAs("_healthSlider")] [SerializeField] private Slider healthSlider; 
    [FormerlySerializedAs("_maxHealth")] [SerializeField] private int maxHealth = 100;

    private int _currentHealth; 

    private void Start()
    {
        InitializeHealthUI();
        SetCurrentHealth(maxHealth);
    }
    

    private void InitializeHealthUI()
    {
        // Ensure the slider is properly set up
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth; 
            healthSlider.value = maxHealth; 
        }
        else
        {
            Debug.LogError("Health Slider is not assigned!");
        }
    }

    public void SetCurrentHealth(int health)
    {
        _currentHealth = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }

    public void ModifyHealth(int amount)
    {
        SetCurrentHealth(_currentHealth + amount);
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = _currentHealth; 
        }
        else
        {
            Debug.LogError("Health Slider is no assigned!");
            //test
        }
    }
}
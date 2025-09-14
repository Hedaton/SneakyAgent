using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health health;
    public Slider healthSlider;

    private void Start()
    {
        health.OnHealthChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar(float current, float max)
    {
        healthSlider.value = current / max;
    }
}

using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public bool destroyOnDeath = true;
    
    [SerializeField] private float maxHealth = 100f;
    public float CurrentHealth { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead()) return;

        CurrentHealth -= damage;

        if(CurrentHealth < 0f)
        {
            CurrentHealth = 0f;
        }

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if(CurrentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        
    }

    public void Heal(float amount)
    {
        if (IsDead()) return;
        CurrentHealth += amount;
        
        CurrentHealth = MathF.Min(CurrentHealth + amount, maxHealth);
    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        CurrentHealth = maxHealth;
    }

    private bool IsDead()
    {
        return CurrentHealth <= 0;
    }
}

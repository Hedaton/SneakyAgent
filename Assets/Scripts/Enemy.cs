using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    public float currentHealth;

    private void Start()
    {
        currentHealth = enemyData.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}

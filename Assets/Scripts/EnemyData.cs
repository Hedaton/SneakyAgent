using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float maxHealth;
    public float moveSpeed;
    public bool isRanged;
    public float detectionRange;
    public float attackRange;
    public float attackCooldown;
}

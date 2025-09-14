using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public bool isRanged;
    public float detectionRange = 20f;
    public float attackRange = 10f;
    public float attackCooldown;
    public float fireSpread = 5f;

    [Header("Wandering Behavior")]
    [Tooltip("Düşmanın dolaşırken bir noktada ne kadar bekleyeceğini belirleyen aralık (min, max).")]
    public Vector2 idleTimeRange = new Vector2(1f, 5f);
    [Tooltip("Dolaşırken gideceği noktanın ne kadar uzakta olacağı.")]
    public float wanderRadius = 15f;

    [Header("Combat Behavior")]
    [Tooltip("Düşmanın bir seferde kaç saniye boyunca ateş edeceğini belirleyen aralık (min, max).")]
    public Vector2 burstDurationRange = new Vector2(0.5f, 2f);
    [Tooltip("Ateş ettikten sonra veya şarjör değiştirirken ne kadar hızlı pozisyon alacağı.")]
    public float repositionSpeedMultiplier = 2f;
    [Tooltip("Pozisyon değiştirirken ne kadar uzağa gideceği (metre cinsinden).")]
    public float repositionRadius = 3f;
}

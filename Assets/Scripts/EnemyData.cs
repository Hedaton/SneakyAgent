using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float maxHealth;
    public float speed;
    public float damage;
    public float range;
    public WeaponType weaponType;
    
    public enum WeaponType
    {
        Melee,
        Ranged
    }
}

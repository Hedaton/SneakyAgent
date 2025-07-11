using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public float fireRate;
    public float damage;
    public int maxAmmo;
    public float reloadDuration;
    public float range;
    
}

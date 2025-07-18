using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Item Data/WeaponData")]
public class WeaponData : ItemData
{
    public float fireRate;
    public float damage;
    public int maxAmmo;
    public float reloadDuration;
    public float range;
}

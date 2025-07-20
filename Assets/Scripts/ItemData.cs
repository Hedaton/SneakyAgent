using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite itemIcon;
    public GameObject itemPrefab;
}
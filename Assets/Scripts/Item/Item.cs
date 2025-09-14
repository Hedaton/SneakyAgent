using UnityEngine;

public class Item : MonoBehaviour, IPickable
{
    public ItemData itemData;

    public void PickUp(InventorySystem inventorySystem)
    {
        inventorySystem.PickUp(gameObject);
    }

    public void Interact()
    {

    }
}
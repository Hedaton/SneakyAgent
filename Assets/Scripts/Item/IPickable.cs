using UnityEngine;

public interface  IPickable : IInteractable
{
    void PickUp(InventorySystem inventorySystem);
}
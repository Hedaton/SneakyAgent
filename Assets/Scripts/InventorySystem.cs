using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

public class InventorySystem : MonoBehaviour
{
    public Transform hand;
    public List<GameObject> inventory = new List<GameObject>();
    public int inventoryCapacity = 5;
    public GameObject equippedItem;
    public bool isHolding;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && equippedItem != null)
            Drop(equippedItem);

        for (int i = 0; i < inventoryCapacity; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && i < inventory.Count)
            {
                EquipItem(inventory[i]);
            }
        }
    }

    public void PickUp(GameObject item)
    {
        if (inventory.Count >= inventoryCapacity)
            return;
        inventory.Add(item);
        BoxCollider[] colliders = item.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in colliders)
        {
            collider.enabled = false;
        }
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        EquipItem(item);
    }

    public void EquipItem(GameObject itemToEquip)
    {
        if (equippedItem == itemToEquip)
            return;
        if (equippedItem != null)
        {
            IEquippable oldItem = equippedItem.GetComponent<IEquippable>();
            if(oldItem != null)
            {
                oldItem.OnUnequip();
            }
            equippedItem.SetActive(false);
            
        }
        equippedItem = itemToEquip;
        equippedItem.SetActive(true);

        IEquippable newItem = equippedItem.GetComponent<IEquippable>();
        if(newItem != null)
        {
            newItem.OnEquip();
        }
        itemToEquip.transform.SetParent(hand);
        itemToEquip.transform.localPosition = Vector3.zero;
        itemToEquip.transform.localRotation = Quaternion.identity;
    }

    public void Drop(GameObject item)
    {
        inventory.Remove(item);
        item.transform.SetParent(null);
        IEquippable oldItem = equippedItem.GetComponent<IEquippable>();
        if (oldItem != null)
        {
            oldItem.OnUnequip();
        }
        BoxCollider[] colliders = item.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in colliders)
        {
            collider.enabled = enabled;
        }
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
        }
        item.SetActive(true);
        equippedItem = null;
    }
}

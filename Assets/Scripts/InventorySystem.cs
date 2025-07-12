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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && equippedItem != null)
            Drop(equippedItem);

        for (int i = 0; i < inventoryCapacity; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (i < inventory.Count)
                {
                    EquipItem(inventory[i]);
                }
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
            collider.gameObject.SetActive(false);
        }
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        EquipItem(item);
    }

    public void EquipItem(GameObject item)
    {
        if (equippedItem != null)
        {
            equippedItem.SetActive(false);
        }
        equippedItem = item;
        equippedItem.SetActive(true);
        item.transform.SetParent(hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    public void Drop(GameObject item)
    {
        inventory.Remove(item);
        item.transform.SetParent(null);
        BoxCollider[] colliders = item.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in colliders)
        {
            collider.gameObject.SetActive(true);
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

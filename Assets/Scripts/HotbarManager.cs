using System.Collections.Generic;
using System.IO.IsolatedStorage;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public Transform slotsParent;
    public GameObject slotPrefab;

    private List<GameObject> slots = new List<GameObject>();

    private void OnEnable()
    {
        InventorySystem.OnInventoryChanged += RefreshHotbar;
        InventorySystem.OnEquipmentChanged += HighlightCorrectSlot;
    }

    private void OnDisable()
    {
        InventorySystem.OnInventoryChanged -= RefreshHotbar;
        InventorySystem.OnEquipmentChanged -= HighlightCorrectSlot;
    }

    private void RefreshHotbar()
    {
        foreach(GameObject slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();

        if (inventorySystem == null || inventorySystem.inventory.Count == 0) return;

        foreach (GameObject inventoryItem in inventorySystem.inventory)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotsParent);

            if(inventoryItem.TryGetComponent<Item>(out Item item))
            {
                Transform iconTransform = newSlot.transform.Find("ItemIcon");
                if(iconTransform != null && iconTransform.TryGetComponent<Image>(out Image itemIconImage))
                {
                    itemIconImage.sprite = item.itemData.itemIcon;
                    itemIconImage.enabled = true;
                }
            }
            slots.Add(newSlot);
        }


        HighlightCorrectSlot();

    }

    private void HighlightCorrectSlot()
    {
        if (inventorySystem == null) return;

        int equippedItemIndex = -1;
        if(inventorySystem.equippedItem != null)
        {
            equippedItemIndex = inventorySystem.inventory.IndexOf(inventorySystem.equippedItem);
        }

        for (int i = 0; i < slots.Count; i++)
        {
            Transform borderTransform = slots[i].transform.Find("SelectionBorder");
            if(borderTransform != null)
            {
                borderTransform.gameObject.SetActive(i == equippedItemIndex);
            }
        }
    }

}

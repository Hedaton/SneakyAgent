using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI ammoDisplay;
    public Camera mainCamera;
    public Camera camera1;
    public Transform panel;
    public GameObject crosshair;

    public TextMeshProUGUI text;

    [Header("Hotbar")]
    public List<Image> hotbarItemIcons;
    public List<GameObject> hotbarHighlits;
    public InventorySystem inventorySystem;

    public void Panel()
    {
        if (mainCamera.gameObject.activeSelf == true)
        {
            text.text = "Main Camera";
            mainCamera.gameObject.SetActive(false);
            camera1.gameObject.SetActive(true);
            panel.gameObject.SetActive(true);
            crosshair.gameObject.SetActive(false);
        }
        else
        {
            text.text = "Second Camera";
            mainCamera.gameObject.SetActive(true);
            camera1.gameObject.SetActive(false);
            panel.gameObject.SetActive(false);
            crosshair.gameObject.SetActive(true);
        }
    }

    public void UpdateAmmoDislay(int currentAmmo, int maxAmmo)
    {
        ammoDisplay.text = currentAmmo + "/" + maxAmmo;
    }

    public void AmmoDisplay(bool show)
    {
        ammoDisplay.enabled = show;
    }

    public void UpdateHotbarUI()
    {
        for (int i = 0; i < hotbarItemIcons.Count; i++)
        {
            if (i < inventorySystem.inventory.Count)
            {
                GameObject itemObject = inventorySystem.inventory[i];
                Item itemComponent = itemObject.GetComponent<Item>();
                if(itemComponent != null && itemComponent.itemData != null)
                {
                    hotbarItemIcons[i].sprite = itemComponent.itemData.itemIcon;
                    hotbarItemIcons[i].enabled = true;
                }

            }
            else
            {
                hotbarItemIcons[i].sprite = null;
                hotbarItemIcons[i].enabled = false;
            }
        }
    }

    public void UpdateHotbarHighlight(int index)
    {
        for (int i = 0; i < hotbarHighlits.Count; i++)
        {
            if (i == index)
            {
                hotbarHighlits[i].SetActive(true);
            }
            else
            {
                hotbarHighlits[i].SetActive(false);
            }
        }
    }

}

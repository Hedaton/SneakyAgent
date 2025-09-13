using System;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public Camera fpsCamera;
    public Transform hand;
    public float interactionRange = 3f;

    public static event Action toggleTeleport;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if(Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, interactionRange))
            {
                if (hit.collider.CompareTag("Pickable"))
                {

                    Item item = hit.collider.GetComponent<Item>();
                    if (item != null)
                    {
                        inventorySystem.PickUp(hit.collider.gameObject);
                    }
                }
                else if (hit.collider.CompareTag("Ammo"))
                {
                    AmmoItem ammoItem = hit.collider.GetComponent<AmmoItem>();
                    if (ammoItem != null)
                    {
                        WeaponController weaponController = hand.GetComponentInChildren<WeaponController>();
                        if (weaponController != null)
                        {
                            weaponController.AddAmmo(ammoItem.amount);
                            Destroy(hit.collider.gameObject);
                        }
                    }
                }
            }
        }   

        if(Input.GetKeyDown(KeyCode.I))
        {
            toggleTeleport?.Invoke();
        }
    }
}

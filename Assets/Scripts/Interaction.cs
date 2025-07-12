using UnityEngine;

public class Interaction : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public Camera fpsCamera;
    public Transform hand;
    public float interactionRange = 3f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if(Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, interactionRange))
            {
                if (hit.collider.CompareTag("Ammo"))
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
                else if (hit.collider.CompareTag("Weapon"))
                {

                    WeaponController weapon = hit.collider.GetComponent<WeaponController>();
                    if (weapon != null)
                    {
                        inventorySystem.PickUp(hit.collider.gameObject);
                    }
                }

                

            }
        }
    }
}

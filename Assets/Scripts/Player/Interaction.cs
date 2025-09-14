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
        if(DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DialogueManager.Instance.DisplayNextSentence();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, interactionRange))
            {
                IPickable pickable = hit.collider.GetComponent<IPickable>();
                if (pickable != null)
                {
                    pickable.PickUp(inventorySystem);
                }

                IAmmo ammo = hit.collider.GetComponent<IAmmo>();
                if (ammo != null)
                {
                    WeaponController weaponController = hand.GetComponentInChildren<WeaponController>();
                    if (weaponController != null)
                    {
                        weaponController.AddAmmo(ammo.GetAmount());
                        Destroy(hit.collider.gameObject);
                    }
                }

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }


            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            toggleTeleport?.Invoke();
        }
    }
}

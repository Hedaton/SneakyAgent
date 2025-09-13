using System;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform player;
    public Transform teleportLocation;

    private Vector3 lastPosition;
    private bool teleported = false;
    private Rigidbody rb;

    public static event Action teleportEvent;

    private void Start()
    {
        rb = player.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Interaction.toggleTeleport += ToggleTeleport;
    }

    private void OnDisable()
    {
        Interaction.toggleTeleport -= ToggleTeleport;
    }

    void ToggleTeleport()
    {
        if (!teleported)
        {
            teleported = true;
            lastPosition = player.position;
            rb.MovePosition(teleportLocation.position);
            teleportEvent?.Invoke();
        }
        else
        {
            teleported = false;
            rb.MovePosition(lastPosition);
            teleportEvent?.Invoke();
        }
    }
}

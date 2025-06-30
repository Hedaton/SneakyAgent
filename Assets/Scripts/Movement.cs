using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float airMultiplier = 0.4f;
    public float jumpForce = 12f;
    public float mouseSensivity = 100f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 mouseInput;
    private float xRotation = 0f;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

    }

    private void GetInputs()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * air)
    }
}

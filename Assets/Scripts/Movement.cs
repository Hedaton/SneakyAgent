using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 100f;

    public Transform playerCamera;
    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    private SprintController sprint;
    private float xRotation = 0f;
    private bool isGrounded;
    private bool cursorIsLocked = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sprint = GetComponent<SprintController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Jump(); 

        if (sprint != null)
        {
            sprint.UpdateSprint();
        }
    }
    private void LateUpdate()
    {
        Looking();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = playerCamera.gameObject.activeSelf
            ? transform.right * x + transform.forward * z
            : Vector3.forward * z + Vector3.right * x;

        float speed = sprint != null ? sprint.CurrentSpeed : moveSpeed;

        if (sprint != null)
            sprint.IsMoving = moveDirection.magnitude > 0.01f;

        Vector3 newVelocity = new Vector3(moveDirection.x * speed, rb.linearVelocity.y, moveDirection.z * speed);
        rb.linearVelocity = newVelocity;

    }

    void Looking()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cursorIsLocked = !cursorIsLocked;
        }

        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (playerCamera.gameObject.activeSelf)
            {
                Look();

            }
            else
            {
                return;
            }

        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
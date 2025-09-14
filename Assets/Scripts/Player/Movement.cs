using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float normalSpeed = 5f;
    public float sprintMultiplier = 1.8f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 100f;

    [Header("Stamina Settings")]
    public float maxStamina = 5f;
    public float staminaDrainRate = 1f;
    public float staminaRegenRate = 1.35f;
    public float staminaRegenRateIdle = 2f;
    public float regenDelay = 3f;
    public float currentStamina;

    [Header("Component References")]
    public Transform playerCamera;
    public Transform groundCheck;
    public LayerMask groundMask;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool cursorIsLocked = true;
    private float regenTimer = 0f;

    [Header("State")]
    public bool isSprinting { get; private set; }
    public bool isMoving { get; private set; }
    private bool canAct = true;

    public float CurrentSpeed
    {
        get { return (isSprinting && currentStamina > 0) ? normalSpeed * sprintMultiplier : normalSpeed; }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentStamina = maxStamina;
    }

    private void OnEnable()
    {
        DialogueManager.onDialogueStart += DisableActions;
        DialogueManager.onDialogueEnd += EnableActions;
    }


    private void OnDisable()
    {
        DialogueManager.onDialogueStart -= DisableActions;
        DialogueManager.onDialogueEnd -= EnableActions;
    }

    void Update()
    {
        if (!canAct) return;

        HandleInput();
        HandleStamina();
    }

    void HandleInput()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);

        if (wantsToSprint && isMoving && currentStamina > 0)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    void HandleStamina()
    {
        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            regenTimer = 0f;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
            }
        }
        else
        {
            if (regenTimer < regenDelay)
            {
                regenTimer += Time.deltaTime;
            }
            else
            {
                RegenStamina();
            }
        }
    }

    void FixedUpdate()
    {
        if (!canAct)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Move();
    }

    void LateUpdate()
    {
        if (!canAct) return;

        Looking();
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = playerCamera.gameObject.activeSelf
            ? transform.right * x + transform.forward * z
            : Vector3.forward * z + Vector3.right * x;

        float currentSpeed = CurrentSpeed;

        Vector3 newVelocity = new Vector3(moveDirection.x * currentSpeed, rb.linearVelocity.y, moveDirection.z * currentSpeed);
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
            Look();
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

    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);
    }

    void RegenStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += (isMoving ? staminaRegenRate : staminaRegenRateIdle) * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    private void DisableActions()
    {
        canAct = false;
        isSprinting = false;
        rb.linearVelocity = Vector3.zero;
    }

    private void EnableActions()
    {
        canAct = true;
    }
}
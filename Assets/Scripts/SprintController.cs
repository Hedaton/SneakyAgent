using UnityEngine;

public class SprintController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float normalSpeed = 5f;
    public float sprintMultiplier = 1.8f;

    [Header("Stamina Settings")]
    public float maxStamina = 5f;
    public float staminaDrainRate = 1f;
    public float staminaRegenRate = 1.35f;
    public float staminaRegenRateIdle = 2f;
    public float regenDelay = 3f;

    public float CurrentStamina { get; private set; }
    private float regenTimer = 0f;
    public float StaminaPercentage => CurrentStamina / maxStamina;

    [Header("State")]
    public bool SprintEnabled { get; set; } = true;
    public bool IsSprinting { get; private set; }
    public bool IsMoving { get; set; }

    private void Start()
    {
        CurrentStamina = maxStamina;
    }
    public float CurrentSpeed
    {
        get
        {
            if (SprintEnabled && IsSprinting && CurrentStamina > 0)
                return normalSpeed * sprintMultiplier;
            return normalSpeed;
        }
    }

    public void UpdateSprint()
    {
        bool wantsToSprint = SprintEnabled && Input.GetKey(KeyCode.LeftShift);

        if (wantsToSprint && CurrentStamina > 0 && IsMoving)
        {
            IsSprinting = true;
            DrainStamina();
            regenTimer = 0f;
        }
        else
        {
            IsSprinting = false;
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

    void DrainStamina()
    {
        CurrentStamina -= staminaDrainRate * Time.deltaTime;
        if (CurrentStamina <= 0f)
        {
            CurrentStamina = 0f;
            IsSprinting = false;
            regenTimer = 0f;
        }
    }

    void RegenStamina()
    {
        if (CurrentStamina < maxStamina)
        {
            CurrentStamina += (IsMoving? staminaRegenRate : staminaRegenRateIdle) * Time.deltaTime;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, maxStamina);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Movement movement;
    public Slider staminaBar;

    private void Update()
    {
        staminaBar.value = movement.currentStamina / movement.maxStamina;
    }
}

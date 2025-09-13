using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public SprintController sprint;
    public Slider staminaBar;

    private void Update()
    {
        staminaBar.value = sprint.StaminaPercentage;
    }
}

using UnityEngine;

public class AmmoItem : MonoBehaviour, IAmmo
{
    public int amount = 30;

    public int GetAmount()
    {
        return amount;
    }

    public void Interact()
    {

    }
}
    
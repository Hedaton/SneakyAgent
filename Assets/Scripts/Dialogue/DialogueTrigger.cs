using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    public Dialogue dialogue;

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }
}

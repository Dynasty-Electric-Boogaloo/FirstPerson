using UI;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void Highlight(bool canInteract)
    {
    }

    public virtual void Interact()
    {
    }

    public virtual bool IsInteractable()
    {
        return true;
    }
}

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
    
    public virtual void Break()
    {
        //ajouter son de casse quand on aura le sound system
        
        gameObject.SetActive(false);
    }
}

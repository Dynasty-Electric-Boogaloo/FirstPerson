using System;
using Interactables;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    public UnityEvent onRestore;

    private void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

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

    public virtual void Restore()
    {
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        gameObject.SetActive(true);
        onRestore?.Invoke();
    }

    public virtual InteractionType GetInteractionType()
    {
        return InteractionType.GrabObject;
    }
}

using System;
using Interactables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Material normal;
    [SerializeField] private Material highlight;
    [SerializeField] private Renderer rend;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    public UnityEvent onRestore;

    protected virtual void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    public void Highlight(bool canInteract)
    {
        if(!rend || highlight == null || normal == null)
            return;
        
        rend.material = canInteract ? highlight : normal;
    }

    public virtual void Interact()
    {
        if(TryGetComponent<EventObject>(out var eventObject))
            eventObject.DoEvent();
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

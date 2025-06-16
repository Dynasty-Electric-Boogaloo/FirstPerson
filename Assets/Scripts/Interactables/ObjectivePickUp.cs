using Player;
using UnityEngine;

public class ObjectivePickUp : Interactable
{
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private bool isEvent = true;
    public bool GetIsEvent => isEvent;
    public void PickedUp()
    {
        if(!isEvent)
            gameObject.SetActive(false);

        if (!isEvent)
        {
            InformationManager.SetText("New part of the music box found!", 2);
            ObjectiveManager.AddToFound(this);
            if(!ObjectiveManager.isLast)
                PlayerRoot.SetPosition(teleportPoint.position);
            return;
        }
        
        if (TryGetComponent<EventObject>(out var eventObject))
            eventObject.DoEvent();
        
    }

    public override void Restore()
    {
        if(!ObjectiveManager.isInList(this))
            base.Restore();
        else
            gameObject.SetActive(false);
    }
}

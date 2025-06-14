using Player;
using UnityEngine;

public class ObjectivePickUp : Interactable
{
    [SerializeField] private Door trap;
    [SerializeField] public int indexObjective;
    [SerializeField] private bool isEvent = true;
    public bool GetIsEvent => isEvent;
    public void PickedUp()
    {
        if(!isEvent)
            gameObject.SetActive(false);
        
        if(trap)
            trap.ChangeState();

        if (!isEvent)
        {
            InformationManager.SetText("New part of the music box found!", 2);
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

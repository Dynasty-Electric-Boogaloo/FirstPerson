using Player;
using UnityEngine;

public class ObjectivePickUp : Interactable
{
    [SerializeField] private Door trap;
    public void PickedUp()
    {
        gameObject.SetActive(false);
        if(trap)
            trap.ChangeState();
    }

    public override void Restore()
    {
        base.Restore();
        
    }
}

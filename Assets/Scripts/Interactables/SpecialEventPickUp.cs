using Player;
using UnityEngine;

public class SpecialEventPickUp : Interactable
{
    [SerializeField] private Door trap;
    [SerializeField] public int indexObjective;
    public virtual void PickedUp()
    {
        gameObject.SetActive(false);
        if(trap)
            trap.ChangeState();
    }

    public override void Restore()
    {
        if(indexObjective < PlayerRoot.CurrentIndex)
            base.Restore();
        else
            gameObject.SetActive(false);
    }
}
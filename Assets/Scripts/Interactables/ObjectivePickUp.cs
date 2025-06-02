using Player;
using UnityEngine;

public class ObjectivePickUp : Interactable
{
    [SerializeField] private Door trap;
    [SerializeField] public int indexObjective;
    public void PickedUp()
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

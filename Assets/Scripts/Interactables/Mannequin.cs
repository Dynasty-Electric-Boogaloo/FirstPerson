using Interactables;
using Player;
using UI;
using UnityEngine;

public class Mannequin : Interactable
{

    public override void Interact()
    {
        PlayerRoot.SetIsDancing(!PlayerRoot.GetIsDancing());
        UiManager.SetDance(false);
    }

    public virtual InteractionType GetInteractionType()
    {
        return InteractionType.Mannequin;
    }
}

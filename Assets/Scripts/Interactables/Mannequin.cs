using Interactables;
using Player;
using UI;
using UnityEngine;

public class Mannequin : Interactable
{

    public override void Interact()
    {
        print("get in");
        PlayerRoot.SetIsDancing(true);
        UiManager.SetDance(true);
    }

    public virtual InteractionType GetInteractionType()
    {
        return InteractionType.Mannequin;
    }
}

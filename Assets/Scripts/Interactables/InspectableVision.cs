using Interactables;
using UnityEngine;

public class InspectableVision : Interactable
{
    protected override void Start()
    {
        
    }

    public override void Break()
    {
    }

    public override InteractionType GetInteractionType()
    {
        return InteractionType.Other;
    }
}

using Interactables;
using Player;
using UI;
using UnityEngine;

public class Mannequin : Interactable
{

    public override void Interact()
    {
        PlayerRoot.SetIsDancing(!PlayerRoot.GetIsDancing(), this);
        UiManager.SetDance(false);
        gameObject.SetActive(false);
    }

    public virtual InteractionType GetInteractionType()
    {
        return InteractionType.Mannequin;
    }

    public void Respawn(Vector3 newPos)
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out var hit))
            newPos.y -= hit.transform.position.y;
        transform.position = newPos;
    }
}

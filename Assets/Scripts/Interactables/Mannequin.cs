using Player;
using UI;
using UnityEngine;

namespace Interactables
{
    public class Mannequin : Interactable
    {
        [SerializeField] private Transform cameraPos;
        
        public Transform GetCameraPos() => cameraPos;

        [SerializeField] private LayerMask groundMask;

        public override InteractionType GetInteractionType()
        {
            return InteractionType.Mannequin;
        }

        public void Respawn(Vector3 newPos)
        {
            if (Physics.Raycast(transform.position, -Vector3.up, out var hit, ~groundMask))
                newPos.y = hit.transform.position.y;
            transform.position = newPos;
        }
    }
}

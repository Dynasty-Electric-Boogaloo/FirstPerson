using Player;
using UI;
using UnityEngine;

namespace Interactables
{
    public class Mannequin : Interactable
    {
        [SerializeField] private Transform cameraPos;
        [SerializeField] private Animator animator;
        [SerializeField] private LayerMask groundMask;
        private string _currentPoseIndex;
        
        public Transform GetCameraPos() => cameraPos;
        
        protected override void Start()
        {
            if(animator)
                _currentPoseIndex = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        }

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

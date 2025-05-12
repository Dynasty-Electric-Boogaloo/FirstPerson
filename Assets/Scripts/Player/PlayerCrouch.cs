using System;
using UnityEngine;

namespace Player
{
    public class PlayerCrouch : PlayerBehaviour
    {
        [SerializeField] private LayerMask ceilingMask;
        [SerializeField] private float ceilingHeightCheck;
        [SerializeField] private float standingColliderSize;
        [SerializeField] private float crouchedColliderSize;
        [SerializeField] private float standingMeshSize;
        [SerializeField] private float crouchedMeshSize;
        [SerializeField] private float standingCamHeight;
        [SerializeField] private float crouchedCamHeight;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private Transform meshTransform;
        private Ray _ceilingCheckRay;
        
        private void Update()
        {
            if (!PlayerData.Grounded) return;

            if (PlayerData.Reloading)
            {
                if (!CanUncrouch()) return;
            }
            
            PlayerData.Reloading = PlayerData.PlayerInputs.Controls.Crouch.IsPressed();

            capsuleCollider.height = PlayerData.Reloading ? crouchedColliderSize : standingColliderSize;
            meshTransform.localScale = PlayerData.Reloading ? new Vector3(1, crouchedMeshSize, 1) : new Vector3(1, standingMeshSize, 1);
            PlayerData.CameraHolder.localPosition = Vector3.up * (PlayerData.Reloading ? crouchedCamHeight : standingCamHeight);
        }

        private bool CanUncrouch()
        {
            _ceilingCheckRay.origin = PlayerData.Rigidbody.position;
            _ceilingCheckRay.direction = Vector3.up;

            Debug.DrawLine(_ceilingCheckRay.origin,
                _ceilingCheckRay.origin + _ceilingCheckRay.direction * ceilingHeightCheck);

            return !Physics.SphereCast(_ceilingCheckRay, .45f, ceilingHeightCheck, ceilingMask);
        }
    }
}
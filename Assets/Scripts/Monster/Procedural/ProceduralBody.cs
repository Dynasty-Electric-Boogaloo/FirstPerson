using System;
using UnityEngine;

namespace Monster.Procedural
{
    public class ProceduralBody : MonoBehaviour
    {
        [SerializeField] private ProceduralHead targetHead;
        [SerializeField] private float distance;
        [SerializeField] private float hoverHeight;
        [SerializeField] private float gravity;
        [SerializeField] private float groundCheckLength;
        [SerializeField] private LayerMask groundMask;
        private float _verticalPosition;
        private float _verticalVelocity;

        private void Start()
        {
            _verticalPosition = transform.position.y;
        }

        private void FixedUpdate()
        {
            UpdateGrounding();
            UpdatePosition();
        }

        private void UpdateGrounding()
        {
            var position = transform.position;
            position.y = _verticalPosition;

            var grounded = Physics.Raycast(position, Vector3.down, out var hitInfo, groundCheckLength, groundMask);

            if (!grounded)
            {
                _verticalVelocity -= gravity * Time.fixedDeltaTime;
                _verticalPosition += _verticalVelocity * Time.fixedDeltaTime;
            }
            else
            {
                _verticalVelocity = 0;
                _verticalPosition = hitInfo.point.y + hoverHeight;
            }
        }
        
        private void UpdatePosition()
        {
            var targetPosition = targetHead.GetPointOnCurve(distance);
            targetPosition.y += _verticalPosition;
            if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.z))
                return;
            
            transform.position = targetPosition;
        }
    }
}
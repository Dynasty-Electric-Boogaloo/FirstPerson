using System;
using UnityEngine;

namespace Monster.Procedural
{
    public class ProceduralBody : MonoBehaviour
    {
        [SerializeField] private ProceduralHead targetHead;
        [SerializeField] private ProceduralBody previousBody;
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

        private void Update()
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
                _verticalVelocity -= gravity * Time.deltaTime;
                _verticalPosition += _verticalVelocity * Time.deltaTime;
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
            targetPosition.Position.y += _verticalPosition;
            if (float.IsNaN(targetPosition.Position.x) || float.IsNaN(targetPosition.Position.z))
                return;

            transform.position = targetPosition.Position;
            transform.rotation = targetPosition.Rotation;

            if (!previousBody)
                return;
            
            transform.rotation = Quaternion.LookRotation((previousBody.transform.position - transform.position).normalized, transform.up);
        }
    }
}
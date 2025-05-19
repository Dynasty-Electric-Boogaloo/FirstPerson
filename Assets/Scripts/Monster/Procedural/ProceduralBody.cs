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
        [SerializeField] private float maxFallSpeed;
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
            //UpdatePosition();
        }

        private void UpdateGrounding()
        {
            var position = transform.position;
            position.y = _verticalPosition;

            var grounded = Physics.Raycast(position, Vector3.down, out var hitInfo, groundCheckLength, groundMask);

            if (!grounded)
            {
                _verticalVelocity -= gravity * Time.fixedDeltaTime;
                _verticalVelocity = Mathf.Clamp(_verticalVelocity, -maxFallSpeed, maxFallSpeed);
                _verticalPosition += _verticalVelocity * Time.fixedDeltaTime;

                if (_verticalPosition < -10)
                {
                    _verticalPosition = previousBody._verticalPosition + 1;
                    _verticalVelocity = 0;
                }
            }
            else
            {
                _verticalVelocity = 0;
                _verticalPosition = hitInfo.point.y + hoverHeight;
            }
        }

        public void UpdatePosition(Vector3 position, Quaternion rotation)
        {
            /*var targetPosition = targetHead.GetPointOnCurve(distance);
            if (float.IsNaN(targetPosition.Position.x) || float.IsNaN(targetPosition.Position.z))
                return;*/

            position.y += _verticalPosition;
            transform.position = position;
            transform.rotation = rotation;

            /*if (!previousBody)
                return;
            
            transform.rotation = Quaternion.LookRotation((previousBody.transform.position - transform.position).normalized, transform.up);*/
        }

        public float GetDistance()
        {
            return distance;
        }
    }
}
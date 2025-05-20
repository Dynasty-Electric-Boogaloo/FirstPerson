using UnityEngine;

namespace Monster.Procedural
{
    public class ProceduralBody : MonoBehaviour
    {
        [SerializeField] private ProceduralHead targetHead;
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
                    _verticalPosition = targetHead.transform.position.y + 1;
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
            transform.position = position;
            transform.rotation = rotation;
        }

        public float GetDistance()
        {
            return distance;
        }

        public float GetVerticalPosition()
        {
            return _verticalPosition;
        }
    }
}
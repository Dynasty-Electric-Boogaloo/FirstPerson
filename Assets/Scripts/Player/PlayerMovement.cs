using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : PlayerBehaviour
    {
        [Serializable]
        private struct MovementSpeedConfig
        {
            public float speed;
            public float acceleration;
            public float deceleration;
        }

        [SerializeField] private MovementSpeedConfig walkConfig;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundedGroundCheckLength;
        [SerializeField] private float airborneGroundCheckLength;
        [SerializeField] private float groundOffset;
        [SerializeField] private float groundLerpSpeed;
        [SerializeField] private float gravity;
        [SerializeField] private float maxFallSpeed;
        private bool _grounded;
        private Ray _groundCheckRay;
        private RaycastHit _hitInfo;

        private void Update()
        {
            UpdateMovement();
            UpdateGravity();
        }

        private void UpdateMovement()
        {
            var move = PlayerData.PlayerInputs.Controls.Move.ReadValue<Vector2>();
            var targetMovement = new Vector3(
                move.x * PlayerData.Right.x + move.y * PlayerData.Forward.x, 
                0, 
                move.x * PlayerData.Right.z + move.y * PlayerData.Forward.z) * walkConfig.speed;

            var linearVelocity = PlayerData.Rigidbody.linearVelocity;
            linearVelocity.y = 0;
            var diff = targetMovement - linearVelocity;
            var acceleration = Vector3.Dot(targetMovement, diff) < 0
                ? walkConfig.deceleration
                : walkConfig.acceleration;

            PlayerData.Rigidbody.AddForce(diff * acceleration, ForceMode.Acceleration);
        }

        private void UpdateGravity()
        {
            var groundCheckLength = _grounded ? groundedGroundCheckLength : airborneGroundCheckLength;
            _groundCheckRay.origin = PlayerData.Rigidbody.position;
            _groundCheckRay.direction = Vector3.down;

            _grounded = Physics.SphereCast(_groundCheckRay, .5f, out _hitInfo, groundCheckLength, groundMask);

            if (!_grounded)
            {
                AccelerateGravity();
                return;
            }

            GroundPlayer();
        }

        private void AccelerateGravity()
        {
            var linearVelocity = PlayerData.Rigidbody.linearVelocity;
            linearVelocity.y = Mathf.Max(linearVelocity.y + gravity * Time.deltaTime, maxFallSpeed);
            PlayerData.Rigidbody.linearVelocity = linearVelocity;
        }

        private void GroundPlayer()
        {
            var linearVelocity = PlayerData.Rigidbody.linearVelocity;
            linearVelocity.y = 0;
            PlayerData.Rigidbody.linearVelocity = linearVelocity;

            var position = PlayerData.Rigidbody.position;
            position.y = Mathf.Lerp(position.y, _hitInfo.point.y + groundOffset, groundLerpSpeed * Time.deltaTime);
            PlayerData.Rigidbody.position = position;
        }
    }
}
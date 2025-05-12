using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMovement : PlayerBehaviour
    {
        [Serializable]
        private struct MovementConfig
        {
            public float speed;
            public float acceleration;
            public float deceleration;
            public float groundOffset;
        }

        [SerializeField] private MovementConfig walkConfig;
        [SerializeField] private MovementConfig slowConfig;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundedGroundCheckLength;
        [SerializeField] private float airborneGroundCheckLength;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private float groundLerpSpeed;
        [SerializeField] private float gravity;
        [SerializeField] private float maxFallSpeed;
        private Ray _groundCheckRay;
        private RaycastHit _sphereHitInfo;
        private RaycastHit _rayHitInfo;

        private void Update()
        {
            UpdateMovement();
            UpdateGravity();
        }

        private void UpdateMovement()
        {
            var moveConfig = PlayerData.Reloading || PlayerData.Dancing ? slowConfig : walkConfig;
            var move = PlayerData.PlayerInputs.Controls.Move.ReadValue<Vector2>();

            if (move.sqrMagnitude < 0.01f)
            {
                move = Vector2.zero;
            }
            
            var targetMovement = new Vector3(
                move.x * PlayerData.Right.x + move.y * PlayerData.Forward.x, 
                0, 
                move.x * PlayerData.Right.z + move.y * PlayerData.Forward.z) * moveConfig.speed;

            var linearVelocity = PlayerData.Rigidbody.linearVelocity;
            linearVelocity.y = 0;
            var diff = targetMovement - linearVelocity;
            var acceleration = Vector3.Dot(targetMovement, diff) <= 0
                ? moveConfig.deceleration
                : moveConfig.acceleration;

            PlayerData.Rigidbody.AddForce(diff * acceleration, ForceMode.Acceleration);
        }

        private void UpdateGravity()
        {
            var groundCheckLength = PlayerData.Grounded ? groundedGroundCheckLength : airborneGroundCheckLength;
            _groundCheckRay.origin = PlayerData.Rigidbody.position;
            _groundCheckRay.direction = Vector3.down;

            PlayerData.Grounded = Physics.SphereCast(_groundCheckRay, groundCheckRadius, out _sphereHitInfo, groundCheckLength, groundMask);

            if (!PlayerData.Grounded)
            {
                AccelerateGravity();
                return;
            }

            GroundPlayer(groundCheckLength);
        }

        private void AccelerateGravity()
        {
            var linearVelocity = PlayerData.Rigidbody.linearVelocity;
            linearVelocity.y = Mathf.Max(linearVelocity.y + gravity * Time.deltaTime, maxFallSpeed);
            PlayerData.Rigidbody.linearVelocity = linearVelocity;
        }

        private void GroundPlayer(float groundCheckLength)
        {
            var moveConfig = PlayerData.Reloading || PlayerData.Dancing ? slowConfig : walkConfig;
            var linearVelocity = PlayerData.Rigidbody.linearVelocity;
            linearVelocity.y = 0;
            PlayerData.Rigidbody.linearVelocity = linearVelocity;

            var rayHit = Physics.Raycast(_groundCheckRay, out _rayHitInfo, groundCheckLength + .5f, groundMask);
            var hitInfo = rayHit ? _rayHitInfo : _sphereHitInfo;

            var position = PlayerData.Rigidbody.position;
            position.y = Mathf.Lerp(position.y, hitInfo.point.y + moveConfig.groundOffset, groundLerpSpeed * Time.deltaTime);
            PlayerData.Rigidbody.position = position;
        }
    }
}
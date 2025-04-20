using System;
using Player;
using UnityEngine;

namespace Monster
{
    public class MonsterMovement : MonsterBehaviour
    {
        [Serializable]
        private struct MovementConfig
        {
            public float speed;
            public float acceleration;
            public float deceleration;
        }

        [SerializeField] private MovementConfig walkConfig;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundedGroundCheckLength;
        [SerializeField] private float airborneGroundCheckLength;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private float groundOffset;
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
            var move = MonsterData.TargetPoint - transform.position;
            move.y = 0;
            move.Normalize();

            if (move.sqrMagnitude < 0.01f)
            {
                move = Vector2.zero;
            }
            
            var targetMovement = move * walkConfig.speed;

            var linearVelocity = MonsterData.Rigidbody.linearVelocity;
            linearVelocity.y = 0;
            var diff = targetMovement - linearVelocity;
            var acceleration = Vector3.Dot(targetMovement, diff) <= 0
                ? walkConfig.deceleration
                : walkConfig.acceleration;

            MonsterData.Rigidbody.AddForce(diff * acceleration, ForceMode.Acceleration);
        }

        private void UpdateGravity()
        {
            var groundCheckLength = MonsterData.Grounded ? groundedGroundCheckLength : airborneGroundCheckLength;
            _groundCheckRay.origin = MonsterData.Rigidbody.position;
            _groundCheckRay.direction = Vector3.down;

            MonsterData.Grounded = Physics.SphereCast(_groundCheckRay, groundCheckRadius, out _sphereHitInfo, groundCheckLength, groundMask);

            if (!MonsterData.Grounded)
            {
                AccelerateGravity();
                return;
            }

            GroundPlayer(groundCheckLength);
        }

        private void AccelerateGravity()
        {
            var linearVelocity = MonsterData.Rigidbody.linearVelocity;
            linearVelocity.y = Mathf.Max(linearVelocity.y + gravity * Time.deltaTime, maxFallSpeed);
            MonsterData.Rigidbody.linearVelocity = linearVelocity;
        }

        private void GroundPlayer(float groundCheckLength)
        { ;
            var linearVelocity = MonsterData.Rigidbody.linearVelocity;
            linearVelocity.y = 0;
            MonsterData.Rigidbody.linearVelocity = linearVelocity;

            var rayHit = Physics.Raycast(_groundCheckRay, out _rayHitInfo, groundCheckLength + .5f, groundMask);
            var hitInfo = rayHit ? _rayHitInfo : _sphereHitInfo;

            var position = MonsterData.Rigidbody.position;
            position.y = Mathf.Lerp(position.y, hitInfo.point.y + groundOffset, groundLerpSpeed * Time.deltaTime);
            MonsterData.Rigidbody.position = position;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;
            
            Gizmos.DrawSphere(MonsterData.TargetPoint, 1);
        }
    }
}
using System;
using UnityEngine;

namespace Monster
{
    public class MonsterMovement : MonsterBehaviour
    {
        [Serializable]
        private struct MovementConfig
        {
            public AnimationCurve speed;
            public float acceleration;
            public float deceleration;
        }

        [SerializeField] private MovementConfig patrolConfig;
        [SerializeField] private MovementConfig searchConfig;
        [SerializeField] private MovementConfig chaseConfig;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundedGroundCheckLength;
        [SerializeField] private float airborneGroundCheckLength;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private float groundOffset;
        [SerializeField] private float groundLerpSpeed;
        [SerializeField] private float gravity;
        [SerializeField] private float maxFallSpeed;
        [SerializeField] private float rotationSpeed;
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
            var moveConfig = MonsterData.chasing ? chaseConfig : MonsterData.searching ? searchConfig : patrolConfig;
            
            var move = MonsterData.targetPoint - transform.position;
            move.y = 0;
            move.Normalize();

            if (move.sqrMagnitude < 0.01f)
            {
                move = Vector2.zero;
            }
            
            var targetMovement = move * moveConfig.speed.Evaluate(MonsterData.stateTime);

            var linearVelocity = MonsterData.rigidbody.linearVelocity;
            linearVelocity.y = 0;
            var diff = targetMovement - linearVelocity;
            var acceleration = Vector3.Dot(targetMovement, diff) <= 0
                ? moveConfig.deceleration
                : moveConfig.acceleration;

            MonsterData.rigidbody.AddForce(diff * acceleration, ForceMode.Acceleration);

            if (linearVelocity.magnitude < 0.1f)
                return;
            
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                Quaternion.LookRotation(linearVelocity.normalized, Vector3.up),
                rotationSpeed * Time.deltaTime);

        }

        private void UpdateGravity()
        {
            var groundCheckLength = MonsterData.grounded ? groundedGroundCheckLength : airborneGroundCheckLength;
            _groundCheckRay.origin = MonsterData.rigidbody.position;
            _groundCheckRay.direction = Vector3.down;

            MonsterData.grounded = Physics.SphereCast(_groundCheckRay, groundCheckRadius, out _sphereHitInfo, groundCheckLength, groundMask);

            if (!MonsterData.grounded)
            {
                AccelerateGravity();
                return;
            }

            GroundPlayer(groundCheckLength);
        }

        private void AccelerateGravity()
        {
            var linearVelocity = MonsterData.rigidbody.linearVelocity;
            linearVelocity.y = Mathf.Max(linearVelocity.y + gravity * Time.deltaTime, maxFallSpeed);
            MonsterData.rigidbody.linearVelocity = linearVelocity;
        }

        private void GroundPlayer(float groundCheckLength)
        { ;
            var linearVelocity = MonsterData.rigidbody.linearVelocity;
            linearVelocity.y = 0;
            MonsterData.rigidbody.linearVelocity = linearVelocity;

            var rayHit = Physics.Raycast(_groundCheckRay, out _rayHitInfo, groundCheckLength + .5f, groundMask);
            var hitInfo = rayHit ? _rayHitInfo : _sphereHitInfo;

            var position = MonsterData.rigidbody.position;
            position.y = Mathf.Lerp(position.y, hitInfo.point.y + groundOffset, groundLerpSpeed * Time.deltaTime);
            MonsterData.rigidbody.position = position;
        }
    }
}
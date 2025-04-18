﻿using UnityEngine;

namespace Player
{
    public class PlayerCamera : PlayerBehaviour
    {
        [SerializeField] private float maxVerticalAngle;
        [SerializeField] private float sensitivity;
        [SerializeField] private float fadeoutSpeed;
        [SerializeField] private float maxMovementSpeed;
        [SerializeField] private AnimationCurve horizontalBobbingFrequency;
        [SerializeField] private AnimationCurve horizontalBobbingAmplitude;
        [SerializeField] private AnimationCurve verticalBobbingFrequency;
        [SerializeField] private AnimationCurve verticalBobbingAmplitude; 
        private float _horizontalBobbingTimer;
        private float _verticalBobbingTimer;
        private float _amplitude;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            var velocity = PlayerData.Rigidbody.linearVelocity;
            velocity.y = 0;
            var speed = velocity.magnitude / maxMovementSpeed;
            
            HandleViewRotation();
            HandleBobbing(speed);
            UpdateRotation(speed);
        }

        private void HandleViewRotation()
        {
            var look = PlayerData.PlayerInputs.Controls.Look.ReadValue<Vector2>();
            PlayerData.CameraRotation += look * sensitivity;
            PlayerData.CameraRotation.y = Mathf.Clamp( PlayerData.CameraRotation.y, -maxVerticalAngle, maxVerticalAngle);
            transform.localEulerAngles = new Vector3(0,  PlayerData.CameraRotation.x, 0);
            PlayerData.Forward = transform.forward;
            PlayerData.Right = transform.right;
        }

        private void HandleBobbing(float speed)
        {
            if (!PlayerData.Grounded)
            {
                FadeoutBobbing();
                return;
            }

            DoBobbing(speed);
        }

        private void FadeoutBobbing()
        {
            _horizontalBobbingTimer = Mathf.Lerp(_horizontalBobbingTimer, 0, fadeoutSpeed * Time.deltaTime);
            _verticalBobbingTimer = Mathf.Lerp(_verticalBobbingTimer, 0, fadeoutSpeed * Time.deltaTime);
        }

        private void DoBobbing(float speed)
        {
            _horizontalBobbingTimer += horizontalBobbingFrequency.Evaluate(speed) * Time.deltaTime;
            _verticalBobbingTimer += verticalBobbingFrequency.Evaluate(speed) * Time.deltaTime;
            _horizontalBobbingTimer %= 2 * Mathf.PI;
            _verticalBobbingTimer %= 2 * Mathf.PI;
        }

        private void UpdateRotation(float speed)
        {
            PlayerData.CameraHolder.localEulerAngles = new Vector3(
                -PlayerData.CameraRotation.y + Mathf.Sin(_verticalBobbingTimer) * verticalBobbingAmplitude.Evaluate(speed), 
                Mathf.Sin(_horizontalBobbingTimer) * horizontalBobbingAmplitude.Evaluate(speed), 
                0);
        }
    }
}
using System;
using UnityEngine;

namespace Player
{
    public class PlayerCamera : PlayerBehaviour
    {
        [SerializeField] private float maxVerticalAngle;
        [SerializeField] private float sensitivity;
        private Vector2 _angle;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            var look = PlayerData.PlayerInputs.Controls.Look.ReadValue<Vector2>();
            _angle += look * sensitivity;
            _angle.y = Mathf.Clamp(_angle.y, -maxVerticalAngle, maxVerticalAngle);
            PlayerData.CameraHolder.localEulerAngles = new Vector3(-_angle.y, 0, 0);
            transform.localEulerAngles = new Vector3(0, _angle.x, 0);
            PlayerData.Forward = transform.forward;
            PlayerData.Right = transform.right;
            PlayerData.CameraRotationX = -_angle.y;
        }
    }
}
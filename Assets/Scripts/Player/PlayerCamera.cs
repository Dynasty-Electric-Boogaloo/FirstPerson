using System;
using UnityEngine;

namespace Player
{
    public class PlayerCamera : PlayerBehaviour
    {
        [SerializeField] private float maxVerticalAngle;
        [SerializeField] private float sensitivity;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            var look = PlayerData.PlayerInputs.Controls.Look.ReadValue<Vector2>();
            PlayerData.CameraRotation += look * sensitivity;
            PlayerData.CameraRotation.y = Mathf.Clamp( PlayerData.CameraRotation.y, -maxVerticalAngle, maxVerticalAngle);
            PlayerData.CameraHolder.localEulerAngles = new Vector3(- PlayerData.CameraRotation.y, 0, 0);
            transform.localEulerAngles = new Vector3(0,  PlayerData.CameraRotation.x, 0);
            PlayerData.Forward = transform.forward;
            PlayerData.Right = transform.right;
        }
    }
}
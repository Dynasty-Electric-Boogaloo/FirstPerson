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
            PlayerData.Angle += look * sensitivity;
            PlayerData.Angle.y = Mathf.Clamp(PlayerData.Angle.y, -maxVerticalAngle, maxVerticalAngle);
            PlayerData.CameraHolder.localEulerAngles = new Vector3(-PlayerData.Angle.y, 0, 0);
            transform.localEulerAngles = new Vector3(0, PlayerData.Angle.x, 0);
            PlayerData.Forward = transform.forward;
            PlayerData.Right = transform.right;
        }
    }
}
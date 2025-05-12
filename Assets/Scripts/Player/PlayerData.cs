using UnityEngine;

namespace Player
{
    public class PlayerData
    {
        public PlayerInputs PlayerInputs;
        public Rigidbody Rigidbody;
        public Transform CameraHolder;
        public Vector3 Forward;
        public Vector3 Right;
        public bool Grounded;
        public bool Reloading;
        public bool Dancing;
        public Vector2 CameraRotation;

    }
}
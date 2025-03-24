using System;
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
        public bool Crouched;
        public Vector2 CameraRotation;
    }
}
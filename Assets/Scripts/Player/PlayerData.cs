using UI;
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
        public bool DestroyingMimic;
        public bool IsInMannequin;
        public int currentIndexObjective = 0;
        public Vector2 CameraRotation;
    }
}
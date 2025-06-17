using UI;
using UnityEngine;

namespace Player
{
    public class PlayerData
    {
        public PlayerInputs PlayerInputs;
        public Rigidbody Rigidbody;
        public Transform CameraHolder;
        public Transform CameraTransform;
        public Vector3 Forward;
        public Vector3 Right;
        public bool Grounded;
        public bool Reloading;
        public bool Dancing;
        public bool DestroyingMimic;
        public bool IsInMannequin;
        public bool RedLight;
        public bool IsSpecialLight;
        public bool Holding;
        public bool MusicBoxIsOut;
        public bool Locked;
        public int CurrentIndexObjective = 0;
        public Vector2 CameraRotation;
    }
}
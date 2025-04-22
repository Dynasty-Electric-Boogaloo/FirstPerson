using System;
using Monster;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerRoot : MonoBehaviour
    {
        private static PlayerRoot _instance;
        [SerializeField] private Transform cameraHolder;
        private PlayerData _playerData;
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        public static Vector3 Position => _instance ? _instance.transform.position : Vector3.zero;
        
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            
            _playerData = new PlayerData
            {
                PlayerInputs = new PlayerInputs(),
                Rigidbody = GetComponent<Rigidbody>(),
                CameraHolder = cameraHolder
            };
            _playerData.PlayerInputs.Enable();
            
            var behaviours = GetComponents<PlayerBehaviour>();
            foreach (var behaviour in behaviours)
            {
                behaviour.Setup(_playerData);
            }

            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
            
            _playerData.PlayerInputs.Disable();
        }

        public static void ResetPosition()
        {
            _instance.transform.position = _instance._startPosition;
            _instance.transform.rotation = _instance._startRotation;
            _instance._playerData.Rigidbody.linearVelocity = Vector3.zero;
        }
    }
}
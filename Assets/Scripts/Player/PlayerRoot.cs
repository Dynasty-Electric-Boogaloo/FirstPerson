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
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
            
            _playerData.PlayerInputs.Disable();
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                MonsterNavigation.Alert(transform.position);
            }
        }
    }
}
using System;
using Interactables;
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
        private PlayerMusicBox _musicBox;

        public static Vector3 Position => _instance ? _instance.transform.position : Vector3.zero;
        
        public static int CurrentIndex  =>  _instance ? _instance._playerData.CurrentIndexObjective : -1;
        
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
            _musicBox = GetComponent<PlayerMusicBox>();
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

        public static void SetVisible(bool visible)
        {
            if(_instance)
                _instance.gameObject.SetActive(visible);
        }

        public static void Die()
        {
            ResetPosition();
            if (_instance._musicBox)
                _instance._musicBox.DecreaseState();
        }

        public static bool GetIsDancing() => _instance._playerData.Dancing;
        
        public static void SetIsDancing(bool setOn) => _instance._playerData.Dancing = setOn;
        
        public static bool GetIsDestroying => _instance._playerData.DestroyingMimic;
        
        public static void SetIsDestroying(bool setOn) => _instance._playerData.DestroyingMimic = setOn;
        
        
        public static bool GetIsInMannequin() => _instance._playerData.IsInMannequin;
        
        public static void SetIsInMannequin(bool setOn) => _instance._playerData.IsInMannequin = setOn;
    }
}
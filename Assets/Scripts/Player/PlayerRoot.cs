using System;
using System.Dynamic;
using Interactables;
using Monster;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerRoot : MonoBehaviour
    {
        private static PlayerRoot _instance;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform cameraHolder;
        private PlayerData _playerData;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private PlayerMusicBox _musicBox;
        private PlayerFeedback _playerFeedback;
        private PlayerDance _playerDance;
        private PlayerFlashLight _playerFlashLight;
        private PlayerCamera _playerCamera;
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
                CameraTransform = cameraTransform,
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
            _playerFeedback = GetComponent<PlayerFeedback>();
            _playerDance = GetComponent<PlayerDance>();
            _playerFlashLight = GetComponent<PlayerFlashLight>();
            _playerCamera = GetComponent<PlayerCamera>();
            SetRedLight(false);
        }

        private void Start()
        {
            DanceManager.OnQteOver.AddListener(OnQteOver);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
            
            _playerData.PlayerInputs.Disable();
        }

        private void Update()
        {
            if (transform.position.y < -10)
                MonsterRoot.Die();
        }

        public static void ResetPosition()
        {
            if(_instance == null)
                return;
            
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
            if(_instance == null)
                return;
            
            AudioManager.PlayOneShot(FMODEvents.GetDeath(), Position);
            DanceManager.ForceStopQte();
            SetIsLocked(false);
            _instance._playerFlashLight.Death();
            ResetPosition();
            
            if (_instance._musicBox)
                _instance._musicBox.DecreaseState();
        }

        public static bool GetIsDancing() => _instance && _instance._playerData.Dancing;
        
        public static void SetIsDancing(bool setOn)
        {
            if(_instance)
                _instance._playerData.Dancing = setOn;
        }

        public static void StartQte(bool isMimic = true)
        {
            if (!_instance)
                return;
            
            SetIsLocked(true);
            DanceManager.StartQte(_instance._playerDance, isMimic);
        }
        
        private void OnQteOver(bool win)
        {
            SetIsLocked(false);
        }
        
        public static bool GetIsDestroying => _instance &&_instance._playerData.DestroyingMimic;
        
        public static void SetIsDestroying(bool setOn)
        {
            if(_instance)
                _instance._playerData.DestroyingMimic = setOn;
        }
        
        public static bool GetIsInMannequin =>_instance && _instance._playerData.IsInMannequin;
        
        public static void SetIsInMannequin(bool setOn)
        {
            if(_instance)
                _instance._playerData.IsInMannequin = setOn;
        }
        
        public static void SetRedLight(bool setOn)
        {
            if (!_instance) 
                return;
            
            _instance._playerData.RedLight = setOn;

            if (!setOn) 
                return;
            
            _instance._playerFeedback.GetEnergy();
            BatteryManager.WakeUpBattery();
        }

        public static void SetPosition(Vector3 position)
        {
            if (!_instance) 
                return;
            
            _instance.transform.position = position;
        }
        
        public static void SetCamera(Transform position = null , bool start = false)
        {
            if (!_instance) 
                return;
            if(start)
                _instance._playerCamera.GoToPosition(position);
            else 
                _instance. _playerCamera.ReturnToPosition();
        }
        
        public static bool GetRedLightUnlocked =>_instance && _instance._playerData.RedLight;
        
        public static bool GetIsLocked =>_instance && _instance._playerData.Locked;
        
        public static void SetIsLocked(bool setOn)
        {
            if (!_instance) 
                return;
            
            _instance._playerData.Locked = setOn;
        }
    }
}
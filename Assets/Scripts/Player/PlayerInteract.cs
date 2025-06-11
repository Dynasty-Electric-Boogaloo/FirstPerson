using System;
using Interactables;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerInteract : PlayerBehaviour
    {
        [SerializeField] private float throwForce;
        [SerializeField] private float grabOffset;
        [SerializeField] private float grabHoldOffset;
        [SerializeField] private float grabDistance;
        [SerializeField] private Transform grabPoint;
        [SerializeField] private Transform grabHinge;
        [SerializeField] private float grabSize = 0.1f;
        private Interactable _selectedObject;
        private GrabObject _grabbedObject;
        private Mannequin _mannequin;
        private RaycastHit _raycastHit;
        private PlayerCamera _playerCamera;
        private PlayerDance _playerDance;
        private PlayerMusicBox _playerMusicBox;

        private void Start()
        {
            _playerCamera = GetComponent<PlayerCamera>();
            _playerMusicBox = GetComponent<PlayerMusicBox>();
            _playerDance = GetComponent<PlayerDance>();
        }

        private void Update()
        {
            if (_mannequin)
            {
                if (!PlayerData.PlayerInputs.Controls.Interact.WasPressedThisFrame()) 
                    return;

                _mannequin = null;
                PlayerRoot.SetIsInMannequin(false);
                UiManager.InMannequin(false);
                _playerCamera.ReturnToPosition();
                return;
            }
            if (!_grabbedObject)
            {
                HandleHighlight();

                if (!_selectedObject)
                    return;
                
                
                TryInspect();

                TryInteract();
                return;
            }
            HandleGrabbed();
        }

        private void FixedUpdate()
        {
            HandleGrabPoint();
        }

        private void HandleGrabPoint()
        {
            if (!_grabbedObject)
                return;
            
            grabPoint.localPosition = Vector3.forward * (grabHoldOffset + _grabbedObject.GetBounds().extents.z);
            grabHinge.localRotation = Quaternion.Euler(-PlayerData.CameraRotation.y, 0, 0);
        }

        private void HandleGrabbed()
        {
            UiManager.SetGrab();
            
            if (TryUngrab())
                return;

            if (TryThrow())
                return;
        }

        private void HandleHighlight()
        {
            if(InspectSystem.isOn())
                return;
            
            var ray = new Ray(
                PlayerData.CameraHolder.position + PlayerData.CameraHolder.forward * grabOffset,
                PlayerData.CameraHolder.forward);
            
            var hit = Physics.SphereCast(ray, grabSize, out _raycastHit, grabDistance);

            if (!hit)
            {
                DeselectObject();
                return;
            }

            if (!_raycastHit.transform.TryGetComponent<Interactable>(out var interactable))
            {
                DeselectObject();
                return;
            }

            if (!interactable.IsInteractable())
            {
                DeselectObject();
                return;
            }
            if(!interactable.GetComponent<Mimic>() || PlayerData.RedLight)
                SelectObject(interactable);
        }

        private void TryInteract()
        {
            if (!PlayerData.PlayerInputs.Controls.Interact.WasPressedThisFrame()) 
                return;
            
            _selectedObject.Interact();
            TryExtract();
            
            if(_selectedObject.GetComponent<Mimic>())
                return;

            if (_selectedObject.TryGetComponent<Mannequin>(out var mannequin) && PlayerRoot.GetRedLightUnlocked)
            {
                PlayerRoot.SetIsInMannequin(!PlayerRoot.GetIsInMannequin);
                UiManager.InMannequin(PlayerRoot.GetIsInMannequin);
                _mannequin = mannequin;
                _playerCamera.GoToPosition(mannequin.GetCameraPos());
                PlayerData.Rigidbody.linearVelocity = Vector3.zero;
            }

            if (_selectedObject.TryGetComponent<ObjectivePickUp>(out var objective) )
            {
                objective.PickedUp();

                if (!TryGetComponent<PlayerMusicBox>(out var music) || objective.GetIsEvent)
                    return;
                
                music.IncreaseState();
            }

            if (_selectedObject is not GrabObject grab) 
                return;
            
            if(_playerMusicBox)
                _playerMusicBox.SetMusicBox(false);
            
            grab.Grab(grabPoint);
            _grabbedObject = grab;
        }
        
        private void TryInspect()
        {
            if (!PlayerData.PlayerInputs.Controls.Inspect.WasPressedThisFrame()) 
                return;
            
            if (InspectSystem.isOn())
            {
                InspectSystem.Hide();
                return;
            }
            
            if (!_selectedObject.TryGetComponent<Inspectable>(out var inspectable)) 
                return;

            inspectable.Inspect();
        }

        private void TryExtract()
        {
            if (!_selectedObject.TryGetComponent<Mimic>(out var mimic) || !PlayerData.RedLight) 
                return;
            
            _playerDance.SetCurrentMimic(mimic);
            _playerDance.SetDancing();

            if (_selectedObject.GetComponent<Mannequin>())
            {
                mimic.SetInfected(false);
                return;
            }
            _selectedObject.Break();
        }

        private bool TryUngrab()
        {
            if (!PlayerData.PlayerInputs.Controls.Interact.WasPressedThisFrame())
                return false;
            
            _grabbedObject.Ungrab();
            _grabbedObject = null;
            return true;
        }

        private bool TryThrow()
        {
            if (!PlayerData.PlayerInputs.Controls.Throw.WasPressedThisFrame())
                return false;

            var throwVelocity = PlayerData.CameraHolder.forward * throwForce;
            _grabbedObject.Throw(throwVelocity);
            _grabbedObject = null;
            return true;
        }

        private void SelectObject(Interactable interactable)
        {
            if (_selectedObject != null)
                _selectedObject.Highlight(false);
            
            _selectedObject = interactable;
            _selectedObject.Highlight(interactable);
            
            UiManager.SetInteract(interactable);
        }

        private void DeselectObject()
        {
            if (_selectedObject == null)
                return;
            
            _selectedObject.Highlight(false);
            _selectedObject = null;
            UiManager.SetInteract(_selectedObject);
        }
    }
}
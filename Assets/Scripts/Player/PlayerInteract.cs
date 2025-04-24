using NUnit.Framework;
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
        private Mimic _mimic;
        private RaycastHit _raycastHit;

        private void Update()
        {
            if (!_grabbedObject)
            {
                HandleHighlight();
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
            if (TryUngrab())
                return;

            if (TryThrow())
                return;
        }

        private void HandleHighlight()
        {
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

            SelectObject(interactable);
        }

        private void TryInteract()
        {
            if (UiManager.UserInterface)
                UiManager.UserInterface.CanInteract(_selectedObject);


            if (PlayerData.PlayerInputs.Controls.Interact.WasPressedThisFrame())
            {
                _selectedObject.Interact();
                
                if (_selectedObject.TryGetComponent(out _mimic)) 
                    _mimic.WakingUp();

                if (_selectedObject is not GrabObject grab) 
                    return;
                
                grab.Grab(grabPoint);
                _grabbedObject = grab;
            }

            if (!PlayerData.PlayerInputs.Controls.Extract.WasPressedThisFrame()) 
                return;
            
            //QTE
            if (_selectedObject.TryGetComponent(out _mimic))
                _mimic.DestroyMimic();
                
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
        }

        private void DeselectObject()
        {
            if (_selectedObject == null)
                return;
            
            _selectedObject.Highlight(false);
            _selectedObject = null;
        }
    }
}
using UnityEngine;

namespace Player
{
    public class PlayerGrab : PlayerBehaviour
    {
        [SerializeField] private float throwForce;
        [SerializeField] private float grabOffset;
        [SerializeField] private float grabDistance;
        [SerializeField] private Transform grabPoint;
        [SerializeField] private Transform grabHinge;
        [SerializeField] private float grabSize = 0.1f;
        private GrabObject _selectedObject;
        private bool _grabbed;
        private RaycastHit _raycastHit;

        private void Update()
        {
            if (!_grabbed)
            {
                HandleHighlight();
                TryGrab();
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
            if (!_selectedObject)
                return;
            
            grabPoint.localPosition = Vector3.forward * (grabOffset + _selectedObject.GetBounds().extents.z);
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

            if (!_raycastHit.transform.TryGetComponent<GrabObject>(out var grabObject))
            {
                DeselectObject();
                return;
            }

            if (grabObject.IsThrown)
            {
                DeselectObject();
                return;
            }

            SelectObject(grabObject);
        }

        private void TryGrab()
        {
            if (!_selectedObject)
                return;

            if (PlayerData.PlayerInputs.Controls.Interact.WasPressedThisFrame())
            {
                _selectedObject.Interact();
                return;
            }
            
            if (!PlayerData.PlayerInputs.Controls.Grab.WasPressedThisFrame())
                return;
            
            _selectedObject.Grab(grabPoint);
            _grabbed = true;
        }
        
        private bool TryUngrab()
        {
            if (!PlayerData.PlayerInputs.Controls.Grab.WasPressedThisFrame())
                return false;
            
            _selectedObject.Ungrab();
            _grabbed = false;
            return true;
        }

        private bool TryThrow()
        {
            if (!PlayerData.PlayerInputs.Controls.Throw.WasPressedThisFrame())
                return false;

            var throwVelocity = PlayerData.CameraHolder.forward * throwForce;
            _selectedObject.Throw(throwVelocity);
            _grabbed = false;
            return true;
        }

        private void SelectObject(GrabObject grabObject)
        {
            if (_selectedObject != null)
            {
                _selectedObject.SetHighlight(false);
            }
            
            _selectedObject = grabObject;
            _selectedObject.SetHighlight(true);
        }

        private void DeselectObject()
        {
            if (_selectedObject == null)
                return;
            
            _selectedObject.SetHighlight(false);
            _selectedObject = null;
        }
    }
}
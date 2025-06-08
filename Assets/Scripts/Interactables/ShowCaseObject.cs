using UnityEngine;
using UnityEngine.InputSystem;

public class ShowCaseObject : MonoBehaviour
{
    [SerializeField] private float _sensitivity = 2;
    private Vector3 _mouseReference;
    private Vector3 _rotation;
    private bool _isRotating;
    

    private void OnMouseDrag()
    {
        if (!_isRotating) 
            return;
        
        _rotation.y = -Mouse.current.delta.x.ReadValue() * _sensitivity;
        _rotation.z = Mouse.current.delta.y.ReadValue() *_sensitivity;
            
        //transform.Rotate(_rotation);
        var rotation = transform.localRotation;
        rotation.eulerAngles = rotation.eulerAngles + _rotation;
        transform.localRotation = rotation;
    }

    private void OnMouseDown()
    {
        _isRotating = true;
        _mouseReference = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        _isRotating = false;
    }
}

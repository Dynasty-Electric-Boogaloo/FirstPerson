using UnityEngine;

public class ShowCaseObject : MonoBehaviour
{
    [SerializeField] private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _rotation;
    private bool _isRotating;

    private void OnMouseDrag()
    {
        if (!_isRotating) 
            return;
        
        _rotation.y = -Input.GetAxis("Mouse X") * _sensitivity;
        _rotation.z = Input.GetAxis("Mouse Y") *_sensitivity;
            
        transform.Rotate(_rotation);
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

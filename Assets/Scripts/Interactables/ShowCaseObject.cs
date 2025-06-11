using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactables
{
    public class ShowCaseObject : MonoBehaviour
    {
        [SerializeField] private float sensitivity = 1;

        private void Update()
        {
            if (!Mouse.current.leftButton.isPressed) 
                return;

            transform.Rotate(Vector3.up, -Mouse.current.delta.x.ReadValue() * sensitivity, Space.World);
            transform.Rotate(Vector3.forward, Mouse.current.delta.y.ReadValue() * sensitivity, Space.World);
        }
    }
}

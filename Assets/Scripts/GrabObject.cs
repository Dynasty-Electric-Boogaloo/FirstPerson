using System;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    [Serializable]
    private struct MaterialSet
    {
        public Material normal;
        public Material highlighted;

        public Material GetMaterial(bool highlighted)
        {
            return highlighted ? this.highlighted : normal;
        }
    }
        
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MaterialSet regularMaterialSet;
    private Collider _collider;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform grabPoint)
    {
        SetHighlight(false);
        transform.SetParent(grabPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Ungrab()
    {
        transform.SetParent(null);
        _rigidbody.constraints = RigidbodyConstraints.None;
    }

    public void Throw(Vector3 velocity)
    {
        Ungrab();
        _rigidbody.linearVelocity = velocity;
    }

    public Bounds GetBounds()
    {
        return _collider ? _collider.bounds : default;
    }
        
    public void SetHighlight(bool highlighted)
    {
        meshRenderer.sharedMaterial = regularMaterialSet.GetMaterial(highlighted);
    }
}
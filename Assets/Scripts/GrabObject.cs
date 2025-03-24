using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class GrabObject : MonoBehaviour
{
    [Serializable]
    private struct MaterialSet
    {
        public Material normal;
        public Material highlighted;
        public Material infectedMaterial;

        public Material GetMaterial(bool highlighted, bool infected = false)
        {
            return highlighted ? this.highlighted : infected? infectedMaterial : normal;
        }
    }
        
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MaterialSet regularMaterialSet;
    [SerializeField] private bool isInfected;
    [SerializeField] private float maxTimeBeforeAlert = 5;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private float _timer;
    

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        SetHighlight(false);
        _timer = maxTimeBeforeAlert;
    }

    public void ReduceTime()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
            Debug.Log("Menace alerted...");
    }

    public void Grab(Transform grabPoint)
    {
        if(isInfected)
            Debug.Log("Menace alerted...");
        SetHighlight(false);
        transform.SetParent(grabPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Interact()
    {
        if(isInfected)
        {
            Debug.Log("QTE");
            Debug.Log("More battery");
        }
        Break();
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
        meshRenderer.sharedMaterial = regularMaterialSet.GetMaterial(highlighted, isInfected);
    }

    private void Break()
    {
        //ajouter son de casse quand on aura le sound system
        
        gameObject.SetActive(false);
    }
    
    public bool GetIsInfected() => isInfected;
}
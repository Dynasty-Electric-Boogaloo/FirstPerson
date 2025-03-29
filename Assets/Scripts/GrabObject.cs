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
    [SerializeField] private LayerMask breakableLayers;
    [SerializeField] private LayerMask playerLayer;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private float _timer;
    private bool _isThrown;
    

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

    private void Update()
    {
        if (!_isThrown) 
            return;
        
        var hitColliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, 1, hitColliders, breakableLayers);
        
        if (hitColliders.Length < 1) 
            return;
        
        Break();
    }


    private void FixedUpdate()
    {
        if(!isInfected) 
            return;
        
        var hitColliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, 1, hitColliders, playerLayer);
        
        if (hitColliders.Length < 1) 
            return;
        
        ReduceTime();
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
        _isThrown = true;
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
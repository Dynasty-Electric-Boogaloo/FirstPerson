using System;
using Monster;
using Player;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

public class GrabObject : Interactable
{
    [SerializeField] private LayerMask breakableLayers;
    [SerializeField] private float breakRadius = 0.2f;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private bool _isThrown;
    private Collider[] hitColliders = new Collider[1];

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        Highlight(false);

    }

    private void FixedUpdate()
    {
        BreakOnImpact();
    }

    public override bool IsInteractable()
    {
        return !_isThrown;
    }
    
    public Bounds GetBounds()
    {
        return _collider ? _collider.bounds : default;
    }
    
    private void BreakOnImpact()
    {
        if (!_isThrown) 
            return;
        
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, breakRadius, hitColliders, breakableLayers);
        
        if(numColliders > 0)
            Break();
    }
    
    public void Grab(Transform grabPoint)
    {
        Highlight(false);
        transform.SetParent(grabPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _collider.enabled = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
    
    public void Ungrab()
    {
        transform.SetParent(null);
        _collider.enabled = true;
        _rigidbody.constraints = RigidbodyConstraints.None;
    }

    public void Throw(Vector3 velocity)
    {
        Ungrab();
        _rigidbody.linearVelocity = velocity;
        _isThrown = true;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, breakRadius);
    }
}
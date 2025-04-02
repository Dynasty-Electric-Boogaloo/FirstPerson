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
        public Material infected;

        public Material GetMaterial(bool highlighted, bool infected = false)
        {
            return highlighted ? this.highlighted : infected? this.infected : normal;
        }
    }
        
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MaterialSet regularMaterialSet;
    [SerializeField] private bool isInfected;
    [SerializeField] private float maxTimeBeforeAlert = 5;
    [SerializeField] private LayerMask breakableLayers;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float breakRadius = 0.2f;
    [SerializeField] private float checkPlayerRadius = 1;
    private Collider _collider;
    private Rigidbody _rigidbody;
    private float _timer;
    private bool _isThrown;

    public bool IsThrown => _isThrown;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        SetHighlight(false);
        _timer = maxTimeBeforeAlert;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, checkPlayerRadius);
        
        Gizmos.color = new Color(1, 0, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, breakRadius);
    }

    private void ReduceTime()
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
        _collider.enabled = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void BreakOnImpact()
    {
        if (!_isThrown) 
            return;
        
        var hitColliders = new Collider[1];
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, breakRadius, hitColliders, breakableLayers);
        
        if(numColliders > 0)
            Break();
    }


    private void FixedUpdate()
    {
        AlertCreature();
        BreakOnImpact();
    }

    private void AlertCreature()
    {
        if(!isInfected) 
            return;
        
        var hitColliders = new Collider[1];
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, checkPlayerRadius, hitColliders, playerLayer);
        
        if (numColliders > 0) 
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
        _collider.enabled = true;
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
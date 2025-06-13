using System;
using FMODUnity;
using Interactables;
using Monster;
using Player;
using UnityEngine;

public class Mimic : MonoBehaviour
{
    [Serializable]
    private struct MaterialSet
    {
        public Material normal;
        public Material revealed;
        public Material awake;
    }
        
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private MaterialSet regularMaterialSet;
    [SerializeField] private bool isInfected;
    [SerializeField] private bool showObject;
    [SerializeField] private float maxTimeBeforeAlert = 15;
    [SerializeField] private float checkPlayerRadius = 1;
    [SerializeField] private LayerMask playerLayer;
    private float _timer;
    private bool _isAwake;
    private Collider[] _hitColliders = new Collider[1];
    private int _numColliders;
    private StudioEventEmitter _emitter;

    private void Awake()
    {
        _timer = maxTimeBeforeAlert;
        meshRenderer.enabled = isInfected || showObject;
        meshRenderer.material = regularMaterialSet.normal;
        
        _emitter = gameObject.AddComponent<StudioEventEmitter>();
        _emitter.EventReference = EventReference.Find("event:/Ambiant/Whispers");
        
        SetInfected(isInfected);

        if (TryGetComponent<Interactable>(out var interactable))
            interactable.onRestore.AddListener(OnRestore);
        
        if (TryGetComponent<Mannequin>(out _))
           MannequinManager.AddToList(this, isInfected);
        
        if(isInfected)
            _emitter.Play();
    }
    
    private void FixedUpdate()
    {
        CheckForPlayer();
    }
    
    private void CheckForPlayer()
    {
        if(!isInfected || _isAwake || PlayerRoot.GetIsDancing() || PlayerRoot.GetIsInMannequin) 
            return;
        
        _numColliders = Physics.OverlapSphereNonAlloc(transform.position, checkPlayerRadius, _hitColliders, playerLayer);
        
        if (_numColliders > 0) 
            ReduceTime();
    }
    
    private void ReduceTime()
    {
        _timer -= Time.deltaTime;

        if (_timer > 0)
            return;
        
        WakingUp();
    }

    private void OnRestore()
    {
        _timer = maxTimeBeforeAlert;
        _isAwake = false;
        SetLightened(false);
    }
    
    public bool GetIsInfected => isInfected;
    
    public void SetLightened(bool inLight)
    {
        if(_isAwake) 
            return;
        
        meshRenderer.sharedMaterial = isInfected && inLight ? regularMaterialSet.revealed : regularMaterialSet.normal;
    }

    public void DestroyMimic()
    {
        if(!BatteryManager.Battery) 
            return;
        
        if(TryGetComponent<Mannequin>(out _))
            MannequinManager.SwitchVessel(this);
        
        BatteryManager.Battery.AddBattery(1);
    }
    
    public void WakingUp()
    {
        if (!isInfected)
            return;
        
        MonsterNavigation.Alert(transform.position);
        _isAwake = true;
        meshRenderer.sharedMaterial = regularMaterialSet.awake;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, checkPlayerRadius);
    }

    public void SetInfected(bool infection)
    {
        isInfected = infection;
        meshRenderer.enabled = isInfected;
        
        if(!_emitter)
            return;
        
        if(isInfected)
            _emitter.Play();
        else
            _emitter.Stop();
    }
}

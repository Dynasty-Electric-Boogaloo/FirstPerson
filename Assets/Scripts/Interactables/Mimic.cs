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
    [SerializeField] private bool props;
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
        _emitter.EventReference = RuntimeManager.PathToEventReference("event:/Ambiant/Whispers");
        _emitter.OverrideMaxDistance = 2.5f;
        _emitter.OverrideAttenuation = true;
        
        SetInfected(isInfected);

        if (TryGetComponent<Interactable>(out var interactable))
            interactable.onRestore.AddListener(OnRestore);
    }

    private void Start()
    {
        MimicManager.AddToList(this, isInfected, TryGetComponent<Mannequin>(out _)); ;
        if(isInfected)
            _emitter.Play();
    }

    private void FixedUpdate()
    {
        CheckForPlayer();
    }
    
    private void CheckForPlayer()
    {
        if(!isInfected || _isAwake || PlayerRoot.GetIsDancing() || PlayerRoot.GetIsInMannequin || props) 
            return;
        
        _numColliders = Physics.OverlapSphereNonAlloc(transform.position, checkPlayerRadius, _hitColliders, playerLayer);
        
        if (_numColliders > 0) 
            ReduceTime();
    }
    
    private void ReduceTime()
    {
        _timer -= Time.deltaTime;

        if (_timer > 0 || props)
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
            MimicManager.SwitchVessel(this);
        
        BatteryManager.Battery.AddBattery(1);
    }
    
    public void WakingUp()
    {
        if (!isInfected)
            return;
        
        if (!_isAwake)
        {
            AudioManager.PlayOneShot(FMODEvents.GetAlert(), transform.position);
        }
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
        meshRenderer.enabled = isInfected || showObject;
        
        if(!_emitter)
            return;
        
        if(isInfected)
            _emitter.Play();
        else
            _emitter.Stop();
    }
}

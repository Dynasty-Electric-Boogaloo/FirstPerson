using System;
using Monster;
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
        
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MaterialSet regularMaterialSet;
    [SerializeField] private bool isInfected;
    [SerializeField] private float maxTimeBeforeAlert = 15;
    [SerializeField] private float checkPlayerRadius = 1;
    [SerializeField] private LayerMask playerLayer;
    private float _timer;
    private bool _isAwake;

    void Awake()
    {
        _timer = maxTimeBeforeAlert;
        meshRenderer.enabled = isInfected;
        meshRenderer.material = regularMaterialSet.normal;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, checkPlayerRadius);
    }
    
    /// <summary>
    /// Change l'apparence des objets en fonction de s'ils sont infecté et dans la lumiere spéciale ou non. 
    /// </summary>
    public void SetLightened(bool inLight)
    {
        if(_isAwake) return;
        meshRenderer.sharedMaterial = isInfected && inLight ? regularMaterialSet.revealed : regularMaterialSet.normal;
    }
    
    private void ReduceTime()
    {
        _timer -= Time.deltaTime;

        if (_timer > 0)
            return;
        
        WakingUp();
        _timer = maxTimeBeforeAlert;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckForPlayer();
    }

    public void DestroyMimic()
    {
        if(!BatteryManager.Battery) return;
        BatteryManager.Battery.AddBattery(1);
    }
    
    public void WakingUp()
    {
        print("wakingUp");
        MonsterNavigation.Alert(transform.position);
        _isAwake = true;
        meshRenderer.sharedMaterial = regularMaterialSet.awake;
        
        //possible sound design ?
    }
    
    public void GetInfected()
    {
        isInfected = true;
        meshRenderer.enabled = isInfected;
    }
    
    private void CheckForPlayer()
    {
        if(!isInfected) 
            return;
        
        var hitColliders = new Collider[1];
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, checkPlayerRadius, hitColliders, playerLayer);
        
        if (numColliders > 0) 
            ReduceTime();
    }
    
    public bool GetIsInfected() => isInfected;
}

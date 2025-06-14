using System;
using NUnit.Framework;
using Player;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

public class BatteryManager : MonoBehaviour
{
    public static BatteryManager Battery;
    private float _currentPower;
    private float _currentBattery;
    [SerializeField] private float maxPowerByBattery;
    [SerializeField] private float maxBattery;
    [UnityEngine.Range(0, 100)]
    [SerializeField] private Hud hud;
    
    private void Awake()
    {
        if (Battery == null) 
            Battery = this;
    }
    
    private void OnDestroy()
    {
        if (Battery == this)
            Battery = null;
    }

    public static void WakeUpBattery()
    {
        Battery._currentBattery = Battery.maxBattery;
        for (var battery = 0; battery < Battery.maxBattery; battery++)
        {
            Battery._currentBattery = battery;
            Battery.hud.UpdateBattery(Battery._currentPower, Battery.maxPowerByBattery, true);
        }
    }

    private void Update()
    {
        if (!PlayerRoot.GetRedLightUnlocked)
        {
            Battery._currentPower = 0;
            for (var battery = 0; battery < maxBattery; battery++)
            {
                Battery._currentBattery = battery+1;
                hud.UpdateBattery(_currentPower, maxPowerByBattery, true);
            }
        }
        
        if(hud)
            hud.UpdateBattery(_currentPower, maxPowerByBattery, true);
        
        if (!PlayerRoot.GetIsInMannequin || !PlayerRoot.GetRedLightUnlocked) 
            return;
        
        UpdateBatteryWithHud();
    }

    public void UpdateBatteryWithHud()
    {
        if(hud && PlayerRoot.GetRedLightUnlocked)
            hud.UpdateBattery(_currentPower, maxPowerByBattery, true);
        ReduceBattery();
    }

    public void AddBattery(float newBattery)
    {
        _currentBattery += newBattery;
        if (_currentBattery > maxPowerByBattery)
            _currentBattery = maxPowerByBattery;
        hud.UpdateBattery(_currentPower, maxPowerByBattery, true);
    }
    
    public void ReduceBattery(float multiplier = 1)
    {
        if(_currentPower > 0)
            _currentPower -= Time.deltaTime * multiplier;

        if (_currentPower > 0) return;
        
        _currentBattery -= 1;
        
        if (_currentBattery <= 0)
            _currentBattery = 0;
        else
            _currentPower = maxPowerByBattery;
    }

    public float GetCurrentPower() => _currentPower; 
    public float GetCurrentPowerMax() => maxPowerByBattery;

    public float GetMaxBattery() => maxBattery;
    public float GetCurrentBattery() => _currentBattery;
}

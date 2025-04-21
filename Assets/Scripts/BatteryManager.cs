using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BatteryManager : MonoBehaviour
{
    public static BatteryManager Battery;
    private float _currentPower;
    private float _currentBattery;
    [SerializeField] private float maxPowerByBattery;
    [SerializeField] private float maxBattery;
    [Range(0, 100)]
    [SerializeField] private int startBatteryPercent;


    private void Awake()
    {
        if (Battery == null) Battery = this;
        else Destroy(this);
    }

    private void Start()
    {
        _currentBattery = maxPowerByBattery * ((float)startBatteryPercent/100);
        _currentPower = maxPowerByBattery * ((float)startBatteryPercent/100);
    }
    
    public void AddBattery(float newBattery)
    {
        _currentBattery += newBattery;
        if (_currentBattery > maxPowerByBattery)
            _currentBattery = maxPowerByBattery;
    }
    
    public void ReduceBattery(float multiplier = 1)
    {
        if(_currentPower > 0)
            _currentPower -= Time.deltaTime * multiplier;

        if (_currentPower > 0) return;
        
        _currentBattery--;
        
        if (_currentBattery <= 0)
            _currentBattery = 0;
        else
            _currentPower = maxPowerByBattery;
    }

    public float GetCurrentBattery() => _currentPower; 
    public float GetCurrentBatteryMax() => maxPowerByBattery;

    public float GetMaxBatteryUnits() => maxBattery;
    public float GetCurrentBatteryUnits() => _currentBattery;
}

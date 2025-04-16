using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class BatteryManager : MonoBehaviour
{
    private float _currentBattery;
    [SerializeField] private float maxBattery;
    [Range(0, 100)]
    [SerializeField] private int startBatteryPercent;


    private void Start()
    {
        _currentBattery = maxBattery * ((float)startBatteryPercent/100);
    }
    
    public void AddBattery(float newBattery)
    {
        _currentBattery += newBattery;
        if (_currentBattery > maxBattery)
            _currentBattery = maxBattery;
    }
    
    public void ReduceBattery(float multiplier = 1)
    {
        if(_currentBattery > 0)
            _currentBattery -= Time.deltaTime * multiplier;

        if (_currentBattery < 0)
            _currentBattery = 0;
    }

    public float GetCurrentBattery() => _currentBattery; 
    public float GetCurrentBatteryMax() => maxBattery; 
}

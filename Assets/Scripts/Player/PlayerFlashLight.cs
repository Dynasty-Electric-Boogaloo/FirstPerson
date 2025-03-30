using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Player
{
    public class PlayerFlashLight : PlayerBehaviour
    {
        [Header("To add in inspector")]
        [SerializeField] private Transform flashLightTransform;
        [SerializeField] private new Light light;
        [SerializeField] private Slider batterySlider;
        
        [Header("Variables")]
        [SerializeField] private float batteryMax;
        [SerializeField] private float addByButtonPressed = 0.25f;
        [SerializeField] private float lightIntensityMultiplier = 5000000;
        
        //Private variables
        private float _battery;
        private bool _isOn;
        private bool _special;
        private PlayerInputs _playerInput;
        private BatteryManager _batteryManager;
        private float CurrentBattery => _special ? _batteryManager.GetCurrentBattery() : _battery;
        private float CurrentBatteryMax  => _special ? _batteryManager.GetCurrentBatteryMax() : batteryMax;
        

        private void Start()
        {
            _playerInput = PlayerData.PlayerInputs; 
            _battery = batteryMax;
            SetLightVisible(false);
            
            if (GetComponent<BatteryManager>())
                _batteryManager = GetComponent<BatteryManager>();
        }

        private void Update()
        {
            CheckSwitch();
            LightUpdate();
        }

        private void LightUpdate()
        {
            if (_playerInput.Controls.UseFlash.WasPressedThisFrame())
               SetLightVisible(!_isOn);
            
            if (_playerInput.Controls.ReloadFlash.IsPressed())
                _battery += addByButtonPressed;
            
            if (_battery > batteryMax)
                _battery = batteryMax;

            if (!_isOn)
                return;
            
            
            if (CurrentBattery > 0)
            {
                if(_special)
                    _battery -= Time.deltaTime;
                else 
                    _batteryManager.ReduceBattery();
                light.intensity = (CurrentBattery / CurrentBatteryMax) * lightIntensityMultiplier;
            }
            
            if(batterySlider)
                batterySlider.value = CurrentBattery / CurrentBatteryMax;
        }

        private void SetLightVisible(bool visible)
        {
            _isOn = visible;
            
            if(!visible)
                light.intensity = 0;
            
            if (batterySlider)
                batterySlider.gameObject.SetActive(_isOn);
        }

        private void CheckSwitch()
        {
            if(!_batteryManager) 
                return;

            if (_playerInput.Controls.UseObject1.WasPressedThisFrame())
                _special = false;
            
            if (_playerInput.Controls.UseObject2.WasPressedThisFrame())
                _special = true;
        }
        
    }
}
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
        private PlayerInputs _playerInput; 

        private void Start()
        {
            _playerInput = PlayerData.PlayerInputs; 
            _battery = batteryMax;
            SetLightVisible(false);
        }

        private void Update()
        {
            LightUpdate();
        }

        private void LightUpdate()
        {
            if (_playerInput.Controls.UseFlash.IsPressed())
               SetLightVisible(!_isOn);
            
            if (_playerInput.Controls.ReloadFlash.IsPressed())
                _battery += addByButtonPressed;
            
            if (_battery > batteryMax)
                _battery = batteryMax;
            
            if (_isOn & _battery > 0)
            {
                _battery -= Time.deltaTime;
                light.intensity = (_battery / batteryMax) * lightIntensityMultiplier;
            }
            
            if(batterySlider)
                batterySlider.value = _battery / batteryMax;
        }

        private void SetLightVisible(bool setOn)
        {
            _isOn = setOn;
            
            if(setOn)
                light.intensity = 0;
            
            if(batterySlider)
                batterySlider.gameObject.SetActive(_isOn);
            
        }

    }
}
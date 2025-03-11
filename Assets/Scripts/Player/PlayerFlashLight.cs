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

        private void Start()
        {
            _battery = batteryMax;
            HideLight();
        }

        private void Update()
        {
            PositionUpdate();
            LightUpdate();
        }

        private void PositionUpdate()
        {
            var angles = flashLightTransform.rotation.eulerAngles;
            angles.x = PlayerData.CameraRotationX;
            var rotation = flashLightTransform.rotation;
            rotation.eulerAngles = angles;
            flashLightTransform.rotation = rotation;
        }

        private void LightUpdate()
        {
            if (PlayerData.PlayerInputs.Controls.UseFlash.IsPressed())
            {
                if(_isOn)
                {
                    _battery += addByButtonPressed;
                    if (_battery > batteryMax) _battery = batteryMax;
                }
                else
                {
                    ShowLight();
                }
            }
            if (PlayerData.PlayerInputs.Controls.HideLight.IsPressed())
            {
                HideLight();
            }
            if (_isOn & _battery > 0)
            {
                _battery -= Time.deltaTime;
                light.intensity = (_battery / batteryMax) * lightIntensityMultiplier;
            }
            if(batterySlider)
            {
                batterySlider.value = _battery / batteryMax;
            }
        }

        private void ShowLight()
        {
            _isOn = true;
            if(batterySlider)
            {
                batterySlider.gameObject.SetActive(true);
            }
        }

        private void HideLight()
        {
            _isOn = false;
            light.intensity = 0;
            if(batterySlider)
            {
                batterySlider.gameObject.SetActive(false);
            }
        }

    }
}
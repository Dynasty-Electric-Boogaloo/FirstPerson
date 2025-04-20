using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerFlashLight : PlayerBehaviour
    {
        [Header("To add in inspector")]
        [SerializeField] private Transform flashLightTransform;
        [SerializeField] private new Light light;
        [SerializeField] private Slider batterySlider;
        [SerializeField] private Image batterySliderColor;
        
        [Header("Variables")]
        [SerializeField] private float batteryMax;
        [SerializeField] private float addByButtonPressed = 0.25f;
        [SerializeField] private float lightIntensityMultiplier = 5000000;
        [SerializeField] private Color lightColor = Color.white;
        [SerializeField] private Color specialLightColor = Color.red;
        [SerializeField] private float maxDistanceHit = 5;
        [SerializeField] private float radiusHit = 5;
        [SerializeField] private LayerMask layerToHit;
        [UnityEngine.Range(-1,1)]
        [SerializeField] private float coneRadius = 0.75f;
        
        //Private variables
        private float _battery;
        private bool _isOn;
        private bool _special;
        private PlayerInputs _playerInput;
        private BatteryManager _batteryManager;
        private float CurrentBattery => _special ? _batteryManager.GetCurrentBattery() : _battery;
        private float CurrentBatteryMax  => _special ? _batteryManager.GetCurrentBatteryMax() : batteryMax;
        private readonly RaycastHit[] _hits = new RaycastHit[10];
        
        private readonly HashSet<GrabObject>[] _lightObjectBuffers = new HashSet<GrabObject>[2];
        private int _bufferSelection;
        
        private HashSet<GrabObject> _lastUpdateLightObjects => _lightObjectBuffers[1 ^ _bufferSelection];
        private HashSet<GrabObject> _currentLightObjects => _lightObjectBuffers[_bufferSelection];
        

        private void Start()
        {
            _playerInput = PlayerData.PlayerInputs; 
            _battery = batteryMax; 
            light.color = lightColor;
            batterySliderColor.color = lightColor;
            SetLightVisible(false);

            _lightObjectBuffers[0] = new HashSet<GrabObject>();
            _lightObjectBuffers[1] = new HashSet<GrabObject>();
            
            if (GetComponent<BatteryManager>())
                _batteryManager = GetComponent<BatteryManager>();
        }

        private void Update()
        {
            CheckSwitch();
            LightUpdate();
            UpdateInfectedObjects();
        }

        private void UpdateInfectedObjects()
        {
            foreach (var prop in _lastUpdateLightObjects)
            {
                if (_currentLightObjects.Contains(prop)) continue;
                prop.SetLightened(false);
            }

            _bufferSelection ^= 1;
            _currentLightObjects.Clear();
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
                if(!_special)
                    _battery -= Time.deltaTime;
                else 
                    _batteryManager.ReduceBattery();
                light.intensity = (CurrentBattery / CurrentBatteryMax) * lightIntensityMultiplier;
            }
            
            if(batterySlider)
                batterySlider.value = CurrentBattery / CurrentBatteryMax;
            
            if (_special)
            {
                var origin = transform.position + transform.forward * radiusHit;
                var size = Physics.SphereCastNonAlloc(origin, radiusHit, transform.forward, _hits, maxDistanceHit-2*radiusHit, layerToHit );
                for (var index = 0; index < size; index++)
                {
                    var normalizedLightToObject = Vector3.Normalize(_hits[index].transform.position - origin);

                    if (Vector3.Dot(transform.forward, normalizedLightToObject) > coneRadius)
                    {
                        var element = _hits[index].transform.GetComponent<GrabObject>();
                        _currentLightObjects.Add(element);
                        element.SetLightened(true);
                    }
                }
            }
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
            {
                _special = false;
                light.color = lightColor;
                batterySliderColor.color = lightColor;
            }
            
            if (_playerInput.Controls.UseObject2.WasPressedThisFrame())
            {
                _special = true;
                light.color = specialLightColor;
                batterySliderColor.color = specialLightColor;
            }
        }

        private void OnDrawGizmosSelected()
        {
            var origin = transform.position + transform.forward * radiusHit;
            Gizmos.color = Color.red;
            for (int i = 0; i < maxDistanceHit; i++)
            {
                Gizmos.DrawWireSphere(origin  , radiusHit);
                origin += transform.forward;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerFlashLight : PlayerBehaviour
    {
        [Header("To add in inspector")]
        [SerializeField] private Transform flashLightTransform;
        [SerializeField] private new Light light;
        [SerializeField] private Hud hud;
        
        [Header("Variables")]
        [SerializeField] private float batteryMax;
        [SerializeField] private float addByButtonPressed = 0.25f;
        [SerializeField] private float lightIntensityMultiplier = 5000000;
        [SerializeField] private Color lightColor = Color.white;
        [SerializeField] private Color specialLightColor = Color.red;
        [SerializeField] private float maxDistanceHit = 5;
        [SerializeField] private float radiusHit = 5;
        [SerializeField] private LayerMask layerToHit;
        [Range(-1,1)]
        [SerializeField] private float coneRadius = 0.75f;
        [SerializeField] private int maxObjectInSight = 10;
        
        //Private variables
        private float _battery;
        private bool _isOn;
        private bool _special;
        private PlayerInputs _playerInput;
        private BatteryManager _batteryManager;
        private float CurrentBattery => _special ? _batteryManager.GetCurrentBattery() : _battery;
        private float CurrentBatteryMax  => _special ? _batteryManager.GetCurrentBatteryMax() : batteryMax;
        private RaycastHit[] _hits = new RaycastHit[10];
        
        private readonly HashSet<GrabObject>[] _lightObjectBuffers = new HashSet<GrabObject>[2];
        private int _bufferSelection;
        
        private HashSet<GrabObject> _lastUpdateLightObjects => _lightObjectBuffers[1 ^ _bufferSelection];
        private HashSet<GrabObject> _currentLightObjects => _lightObjectBuffers[_bufferSelection];
        

        private void Start()
        {
            _playerInput = PlayerData.PlayerInputs;
            _hits = new RaycastHit[maxObjectInSight];
            _battery = batteryMax; 
            light.color = lightColor;
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
                if (_currentLightObjects.Contains(prop)) 
                    continue;
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
            
            if (hud)
                hud.UpdateBattery(CurrentBattery, CurrentBatteryMax);
            
            RevealObjects();
        }

        private void RevealObjects()
        {
            if (!_special) 
                return; ;
            var origin = transform.position + transform.forward * radiusHit;
            var size = Physics.SphereCastNonAlloc(origin, radiusHit, transform.forward, _hits, maxDistanceHit - (2 * radiusHit), layerToHit);
            for (var index = 0; index < size; index++)
            {
                var normalizedLightToObject = Vector3.Normalize(_hits[index].transform.position - origin);

                if (!(Vector3.Dot(transform.forward, normalizedLightToObject) > coneRadius)) 
                    continue;
                
                var element = _hits[index].transform.GetComponent<GrabObject>();
                _currentLightObjects.Add(element);
                element.SetLightened(true);
            }
        }

        private void SetLightVisible(bool visible)
        {
            _isOn = visible;
            
            if(!visible)
                light.intensity = 0;
            
            if (hud)
                hud.SetFlashLight(_special, _isOn);
        }

        private void CheckSwitch()
        {
            if(!_batteryManager) 
                return;

            if (_playerInput.Controls.UseObject1.WasPressedThisFrame())
            {
                _special = false;
                light.color = lightColor;
            }
            
            if (_playerInput.Controls.UseObject2.WasPressedThisFrame())
            {
                _special = true;
                light.color = specialLightColor;
            }
            
            if(hud)
                hud.SetFlashLight(_special, _isOn);
        }

        private void OnDrawGizmosSelected()
        {
            var origin = transform.position + transform.forward * radiusHit;
            Gizmos.color = Color.red;
            for (var i = 0; i < maxDistanceHit; i++)
            {
                Gizmos.DrawWireSphere(origin, radiusHit);
                origin += transform.forward;
            }
        }
    }
}
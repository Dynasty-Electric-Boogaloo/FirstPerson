using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using UI;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;
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
        [SerializeField] [Range(0f, 1f)] private float lightFalloffThreshold = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float flicker = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float flickerWait = 0.05f;
        [SerializeField] private float specialLightIntensityMultiplier = 5000000;
        [SerializeField] private Color lightColor = Color.white;
        [SerializeField] private Color specialLightColor = Color.red;
        [SerializeField] private float maxDistanceHit = 5;
        [SerializeField] private float radiusHit = 5;
        [SerializeField] private LayerMask layerToHit;
        [Range(-1,1)]
        [SerializeField] private float coneRadius = 0.75f;
        [SerializeField] private int maxObjectInSight = 10;
        
        private float _battery;
        private bool _isOn;
        private bool _special;
        private PlayerInputs _playerInput;
        private BatteryManager _batteryManager;
        private float CurrentBattery => _special ? _batteryManager.GetCurrentPower() : _battery;
        private float CurrentBatteryMax  => _special ? _batteryManager.GetCurrentPowerMax() : batteryMax;
        private float CurrentLightIntensity  => _special ? specialLightIntensityMultiplier : lightIntensityMultiplier;
        private RaycastHit[] _hits;

        private readonly HashSet<Mimic>[] _lightObjectBuffers = new HashSet<Mimic>[2];
        private int _bufferSelection;
        
        private HashSet<Mimic> _lastUpdateLightObjects => _lightObjectBuffers[1 ^ _bufferSelection];
        private HashSet<Mimic> _currentLightObjects => _lightObjectBuffers[_bufferSelection];

        private bool isFlickering;
        

        private void Start()
        {
            _playerInput = PlayerData.PlayerInputs;
            _hits = new RaycastHit[maxObjectInSight];
            _battery = batteryMax; 
            light.color = lightColor;
            SetLightVisible(false);

            _lightObjectBuffers[0] = new HashSet<Mimic>();
            _lightObjectBuffers[1] = new HashSet<Mimic>();
            
            if (GetComponent<BatteryManager>())
                _batteryManager = GetComponent<BatteryManager>();

            light.intensity = CurrentLightIntensity;
        }

        private void Update()
        {
            LightUpdate();
            UpdateInfectedObjects();
        }
        private void LightUpdate()
        {
            _playerInput.Controls.UseFlash.performed  +=
                context =>
                {
                    switch (context.interaction)
                    {
                        case SlowTapInteraction:
                            SetLightVisible(!_isOn);
                            break;
                        case TapInteraction when _isOn:
                            CheckSwitch();
                            break;
                        case TapInteraction:
                            SetLightVisible(true);
                            break;
                    }
                };

            var reload = _playerInput.Controls.ReloadFlash.IsPressed();
            PlayerData.Reloading = reload;
            if (reload)
                _battery += addByButtonPressed * Time.deltaTime;

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
                
                if( CurrentBattery / CurrentBatteryMax < lightFalloffThreshold) 
                    light.intensity = ((CurrentBattery  / CurrentBatteryMax ) * CurrentLightIntensity / lightFalloffThreshold ) ;
                

                if (Mathf.Abs((CurrentBattery  / CurrentBatteryMax ) - flicker) < 0.001f && !isFlickering)
                {
                    StartCoroutine(nameof(Flickering));
                }
            }
            
            if (hud)
                hud.UpdateBattery(CurrentBattery, CurrentBatteryMax, _special);
            
            RevealObjects();
        }

        IEnumerator Flickering()
        {
            isFlickering = true;
            for (int i = 0; i < 3; i++)
            {
                light.gameObject.SetActive(false);
                yield return new WaitForSeconds(flickerWait);
                light.gameObject.SetActive(true);
                yield return new WaitForSeconds(flickerWait);
            }
            isFlickering = false;
        }
        
        
        private void RevealObjects()
        {
            if (!_special) 
                return;
            
            var origin = transform.position + transform.forward * radiusHit;
            var size = Physics.SphereCastNonAlloc(origin, radiusHit, transform.forward, _hits, maxDistanceHit - (2 * radiusHit), layerToHit);
            for (var index = 0; index < size; index++)
            {
                var normalizedLightToObject = Vector3.Normalize(_hits[index].transform.position - origin);

                if (Vector3.Dot(transform.forward, normalizedLightToObject) <= coneRadius) 
                    continue;
                
                if(!_hits[index].transform.TryGetComponent<Mimic>(out var mimic)) 
                    continue;
                
                _currentLightObjects.Add(mimic);
                mimic.SetLightened(true);
            }
        }

        private void UpdateInfectedObjects()
        {
            foreach (var prop in _lastUpdateLightObjects)
            {
                if(!prop) 
                    return;
                
                if (_currentLightObjects.Contains(prop)) 
                    continue;
                prop.SetLightened(false);
            }

            _bufferSelection ^= 1;
            _currentLightObjects.Clear();
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

            _special = !_special;
            light.color =  light.color == lightColor ? specialLightColor : lightColor;
            
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
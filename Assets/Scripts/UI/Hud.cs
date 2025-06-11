using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        [Header("To add in inspector")]
        [SerializeField] private Slider batterySlider;
        [SerializeField] private Image batterySliderColor;
        [SerializeField] private List<Animator> eyes;
        
        [Header("Variables")]
        [SerializeField] private Color lightColor = Color.white;

        private void Start()
        {
            SetFlashLight(false, false);
        }

        public void SetFlashLight(bool special, bool isOn)
        {
            if (!batterySlider) 
                return;
            
            batterySlider.gameObject.SetActive(isOn && !special);
            batterySliderColor.color = lightColor;
        }
        
        public void UpdateBattery(float currentBattery, float currentMax, bool special)
        {
            if (!batterySlider) 
                return;

            if (!special)
            {
                batterySlider.value = currentBattery / currentMax;
                return;
            }

            if (BatteryManager.Battery.GetCurrentBattery() > eyes.Count || BatteryManager.Battery.GetCurrentBattery() <= 0) 
                return;
            
            eyes[(int)BatteryManager.Battery.GetCurrentBattery()-1].Play($"ClosingEye", 0, 1 -currentBattery / currentMax);
        }
        
    }
}

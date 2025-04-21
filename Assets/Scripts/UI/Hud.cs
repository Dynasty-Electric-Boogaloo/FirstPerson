using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        [Header("To add in inspector")]
        [SerializeField] private Slider batterySlider;
        [SerializeField] private Image batterySliderColor;
        [SerializeField] private Image batteryImage;
        
        [Header("Variables")]
        [SerializeField] private Color lightColor = Color.white;
        [SerializeField] private Color specialLightColor = Color.red;

        private void Start()
        {
            SetFlashLight(false, false);
        }

        public void SetFlashLight(bool special, bool isOn)
        {
            if (!batterySlider) return;
            batterySlider.gameObject.SetActive(isOn);
            batterySliderColor.color = special ? specialLightColor : lightColor;

            if (special)
            {
                batteryImage.fillAmount = BatteryManager.Battery.GetCurrentBatteryUnits() /
                               BatteryManager.Battery.GetMaxBatteryUnits();
            }

        }

        public void UpdateBattery(float currentBattery, float currentMax)
        {
            if (!batterySlider) return;
            batterySlider.value = currentBattery / currentMax;
        }
        
    }
}

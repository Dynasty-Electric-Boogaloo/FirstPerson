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

        }

        public void UpdateBattery(float currentBattery, float currentMax)
        {
            batterySlider.value = currentBattery / currentMax;
        }
        
    }
}

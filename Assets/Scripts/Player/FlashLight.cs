using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Player
{
    public class FlashLight : PlayerBehaviour
    {
        [SerializeField] private GameObject flashLight;
        [SerializeField] private Light light;
        private float battery;
        [SerializeField] float batteryMax;
        [SerializeField] float addBybuttonPressed = 0.25f;
        private bool isOn;
        [SerializeField] private Slider batterySlider;

        void Start()
        {
            battery = batteryMax;
            HideLight();
        }
        void Update()
        {
            if (PlayerData.PlayerInputs.Controls.UseFlash.IsPressed())
            {
                if(isOn) battery += addBybuttonPressed;
                isOn = true;
                flashLight.SetActive(true);
                if (battery > batteryMax) battery = batteryMax;
                batterySlider.gameObject.SetActive(true);
            }
            if (PlayerData.PlayerInputs.Controls.HideLight.IsPressed()) HideLight();
            if (isOn & battery > 0)
            {
                battery -= Time.deltaTime;
                light.intensity = battery / batteryMax * 3;
            }
            batterySlider.value = battery / batteryMax;
        }
        void HideLight()
        {
            isOn = false;
            light.intensity = 0;
            batterySlider.gameObject.SetActive(false);
            flashLight.SetActive(false);
        }

    }
}
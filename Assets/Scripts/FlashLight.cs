using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : PlayerBehaviour
{
    [SerializeField] private Light light;
    private float battery;
    [SerializeField] float batteryMax;
    private bool isOn;
    void Start()
    {
        battery = batteryMax;
    }
    void Update()
    {
        if(PlayerData.PlayerInputs.Controls.UseFlash.IsPressed())
        {
            isOn = true;
            battery += batteryMax / 10;
            if (battery > batteryMax) battery = batteryMax;
            light.intensity = battery / batteryMax * 3;
        }
        if (PlayerData.PlayerInputs.Controls.HideLight.IsPressed()) HideLight();
        if (isOn & battery > 0) battery -= Time.deltaTime;
    }
    void HideLight()
    {
        isOn = false;
        light.intensity = 0;
    }
    
}

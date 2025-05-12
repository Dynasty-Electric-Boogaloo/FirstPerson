using System;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerDance : PlayerBehaviour
    {
        [SerializeField] private float timeBetween = 1f;
        [SerializeField] [Range(0.0f, 1.0f)] private float tolerance = 0.5f;
        private float _timer;

        private void Start()
        {
            _timer = timeBetween;
        }
        
        private void Update()
        {
            if(PlayerData.Dancing)
            {
                _timer -= Time.deltaTime;

                if (_timer < 0 - timeBetween * tolerance ||
                    (PlayerData.PlayerInputs.Controls.Dance.WasPressedThisFrame() &&
                     _timer > 0 + timeBetween * tolerance))
                    UiManager.SetDance(false);

                if (!PlayerData.PlayerInputs.Controls.Dance.WasPressedThisFrame())
                    return;

                if (PlayerData.Dancing)
                    _timer = timeBetween;
                else
                    SetDancing(true);
            }
            SetDancing(true);
        }

        private void SetDancing(bool setOn = false)
        {
            PlayerData.Dancing = setOn;
            UiManager.SetDance(setOn);
        }
    }
}
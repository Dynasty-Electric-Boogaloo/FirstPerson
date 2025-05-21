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
        private Mannequin _holderMannequin;

        private void Start()
        {
            _timer = timeBetween;
        }
        
        private void Update()
        {
            if(PlayerData.Dancing)
            {
                _timer -= Time.deltaTime;

                if (_timer < 0 - timeBetween * tolerance || (PlayerData.PlayerInputs.Controls.Dance.WasPressedThisFrame() && _timer > 0 + timeBetween * tolerance))
                   SetDancing(false);

                if (PlayerData.IsInMannequin && PlayerData.PlayerInputs.Controls.Dance.WasPressedThisFrame())
                    SetDancing(false);

                if (!PlayerData.PlayerInputs.Controls.Dance.WasPressedThisFrame())
                    return;

                if (PlayerData.Dancing)
                    _timer = timeBetween;
            }
            else if (PlayerData.PlayerInputs.Controls.Dance.WasPressedThisFrame())
            {
                _timer = timeBetween;
                SetDancing(true);
            }
        }

        private void SetDancing(bool setOn )
        {
            if (!setOn && _holderMannequin)
            {
                _holderMannequin.Respawn(transform.position);
                _holderMannequin.gameObject.SetActive(!setOn);
            }
            PlayerData.Dancing = setOn;
            UiManager.SetDance(setOn);
        }

        public void SetHolder(Mannequin holder)
        {
            _holderMannequin = holder;
        }
    }
}
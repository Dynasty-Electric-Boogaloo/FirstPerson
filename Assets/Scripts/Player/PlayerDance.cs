using System;
using UnityEngine;

namespace Player
{
    public class PlayerDance : PlayerBehaviour
    {
        private void Update()
        {
            if (!PlayerData.PlayerInputs.Controls.Dance.WasPressedThisFrame()) 
                return;
            
            SetDancing(!PlayerData.Dancing);
        }

        public void SetDancing(bool setOn = false)
        {
            PlayerData.Dancing = setOn;
        }
    }
}
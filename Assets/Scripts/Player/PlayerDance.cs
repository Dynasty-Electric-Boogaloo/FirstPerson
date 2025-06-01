using System;
using Interactables;
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
            
            if (PlayerData.PlayerInputs.Controls.Dance.WasPressedThisFrame() && !PlayerData.IsInMannequin)
            {
                SetDancing(true);
            }
        }

        public void SetDancing(bool setOn, bool isMimic = false )
        {
            UiManager.SetDance(tolerance, isMimic);
            if(!isMimic)
                PlayerData.Dancing = setOn;
            PlayerData.DestroyingMimic = isMimic;
        }
        
    }
}
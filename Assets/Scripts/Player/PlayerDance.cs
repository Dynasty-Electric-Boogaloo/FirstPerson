using System;
using Interactables;
using Monster;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerDance : PlayerBehaviour
    {
        [SerializeField] private float timeBetween = 1f;
        [SerializeField] private ParticleSystem getEnergy;
        [SerializeField] private ParticleSystem setEnergy;
        private Mimic currentMimic; 
        private float _timer;

        private void Start()
        {
            _timer = timeBetween;
        }

        public void SetDancing( )
        {
            /*
            UiManager.SetDance(tolerance, isMimic);
            
            if(!isMimic)
                PlayerData.Dancing = setOn;
            
            PlayerData.DestroyingMimic = isMimic;
            */
            DanceManager.StartQte(this);
        }

        public void SetQteResult(bool win)
        {
            if(win)
            {
                currentMimic.DestroyMimic();
                if(getEnergy)
                    getEnergy.Play();
                if(setEnergy)
                    setEnergy.Play();
            }
            else
                MonsterNavigation.Alert(transform.position);
            
            QteUiPanel.HideQte();
        }

        public void SetCurrentMimic(Mimic newMimic)
        {
            currentMimic = newMimic;
        }
    }
}
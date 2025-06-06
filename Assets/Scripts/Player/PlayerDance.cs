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
        [SerializeField] [Range(0.0f, 1.0f)] private float tolerance = 0.5f;
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
                currentMimic.DestroyMimic();

            else
                MonsterNavigation.Alert(transform.position);
        }

        public void SetCurrentMimic(Mimic newMimic)
        {
            currentMimic = newMimic;
        }
    }
}
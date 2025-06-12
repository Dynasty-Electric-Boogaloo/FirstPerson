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
        private Mimic _currentMimic; 
        private PlayerFeedback _playerFeedback;

        private void Start()
        {
            _playerFeedback = GetComponent<PlayerFeedback>();
        }

        public void SetDancing( )
        {
            DanceManager.StartQte(this);
        }

        public void SetQteResult(bool win)
        {
            if(win)
            {
                _currentMimic.DestroyMimic();
                _playerFeedback.GetEnergy();
            }
            else
                MonsterNavigation.Alert(transform.position);
            
            QteUiPanel.HideQte();
        }

        public void SetCurrentMimic(Mimic newMimic)
        {
            _currentMimic = newMimic;
        }
        
    }
}
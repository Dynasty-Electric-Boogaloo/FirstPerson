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
        private bool _alreadyDid;

        private void Start()
        {
            _playerFeedback = GetComponent<PlayerFeedback>();
        }

        public void SetDancing( )
        {
            PlayerRoot.StartQte();
            if (!_alreadyDid)
            {
                _alreadyDid = true;
                InformationManager.SetText("Utiliser la touche [Espace] pour danser", 1);
            }
        }

        public void SetQteResult(bool win)
        {
            if(!_currentMimic)
                return;
            
            if(win)
            {
                _currentMimic.DestroyMimic();
                _playerFeedback.GetEnergy();
            }
            else
                MonsterNavigation.Alert(transform.position);
        }

        public void SetCurrentMimic(Mimic newMimic)
        {
            _currentMimic = newMimic;
        }
        
    }
}
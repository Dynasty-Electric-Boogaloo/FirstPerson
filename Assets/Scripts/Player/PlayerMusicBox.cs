using System;
using UnityEngine;

namespace Player
{
    public class PlayerMusicBox : PlayerBehaviour
    {
        [SerializeField] private float minimalDistance;
        [SerializeField] private MusicBoxObject musicBoxObject;
        private bool _isOnDisplay;
        [SerializeField] private State state;

        private void Awake()
        {
            musicBoxObject.gameObject.SetActive(_isOnDisplay);
        }

        private void Start()
        {
            musicBoxObject.SetLevel((int)state);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                _isOnDisplay = !_isOnDisplay;
                musicBoxObject.gameObject.SetActive(_isOnDisplay);
            }

            if (!_isOnDisplay || (Vector3.Distance(Monster.MonsterRoot.GetMonsterPosition(), transform.position) > minimalDistance)) 
                return;

            musicBoxObject.Using(
                Vector3.Distance(Monster.MonsterRoot.GetMonsterPosition() / minimalDistance, transform.position));
        }

        public void IncreaseState()
        {
            if ((int)state >= 3)
                return;
            
            state +=  1;
            musicBoxObject.SetLevel((int)state);
        }
        
        public void DecreaseState()
        {
            if((int)state <= 0)
                return;
            
            state -=  1;
            musicBoxObject.SetLevel((int)state);
        }
    
        [Serializable]
        private enum State
        {
            empty,
            ballerina,
            key,
            picture,
        }
    }
}

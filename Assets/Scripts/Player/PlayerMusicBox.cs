using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMusicBox : PlayerBehaviour
    {
        [SerializeField] private float thresholdDetectionDistance;
        [SerializeField] private MusicBoxObject musicBoxObject;
        [SerializeField] private State state;
        private bool _isOnDisplay;
       
        public bool GetIsOnDisplay() => _isOnDisplay;
        
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
                SetMusicBox(!_isOnDisplay);
            }
            if(!_isOnDisplay)
                return;
            
            BatteryManager.Battery.UpdateBatteryWithHud();

            if ((Vector3.Distance(Monster.MonsterRoot.GetMonsterPosition(), transform.position) > thresholdDetectionDistance)) 
                return;

            musicBoxObject.Using((Monster.MonsterRoot.GetMonsterPosition() - transform.position) / thresholdDetectionDistance, transform.forward);
        }

        public void SetMusicBox(bool setOn)
        {
            _isOnDisplay = setOn;
            musicBoxObject.gameObject.SetActive(setOn);
        }

        public void IncreaseState()
        {
            if ((int)state >= 3)
                return;
            
            state +=  1;
            PlayerData.currentIndexObjective = (int)state;
            musicBoxObject.SetLevel((int)state);
        }
        
        public void DecreaseState()
        {
            if((int)state <= 0)
                return;
            
            state -=  1;
            PlayerData.currentIndexObjective = (int)state;
            musicBoxObject.SetLevel((int)state);
        }
    
        [Serializable]
        private enum State
        {
            Empty,
            Ballerina,
            Key,
            Picture,
        }
    }
}

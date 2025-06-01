using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMusicBox : PlayerBehaviour
    {
        [FormerlySerializedAs("minimalDistance")] [SerializeField] private float thresholdDetectionDistance;
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

            if (!_isOnDisplay || (Vector3.Distance(Monster.MonsterRoot.GetMonsterPosition(), transform.position) > thresholdDetectionDistance)) 
                return;

            musicBoxObject.Using((Monster.MonsterRoot.GetMonsterPosition() - transform.position) / thresholdDetectionDistance, transform.forward);
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

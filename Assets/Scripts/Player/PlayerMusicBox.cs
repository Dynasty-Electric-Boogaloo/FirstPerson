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
                Vector3.Distance(Monster.MonsterRoot.GetMonsterPosition() / minimalDistance, transform.position),
                Monster.MonsterRoot.GetMonsterPosition());
        }

        public void ChangeState(bool more = true)
        {
            state += more ? 1 : -1;
            musicBoxObject.SetLevel((int)state);
        }
    
        [Serializable]
        private enum State
        {
            state0,
            state1,
            state2,
            state3,
        }
    }
}

using System;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
    public class DancePanel : MonoBehaviour
    {
        [SerializeField] private List<int> notePositions = new List<int>();
        [SerializeField] private List<MimicDestructionQte> qteMimic = new List<MimicDestructionQte>();
        [SerializeField] private List<Image> notes = new List<Image>();
        [SerializeField] private List<Image> playerImpulse = new List<Image>();
        [SerializeField] private int maxDistance = 200;
        [SerializeField] private float speed = 0.15f;
        private int _currentIndex;
        private MimicDestructionQte _currentMimicQTE;
        private bool _isDancing;
        private bool _isDestroyingMimic;
        
        private void Update()
        {
            if (!_isDancing) return;
            for (var i = 0; i < playerImpulse.Count; i++)
            {
                if ((playerImpulse[i].rectTransform.anchoredPosition.x >= maxDistance && i%2 == 0 ) ||
                    (playerImpulse[i].rectTransform.anchoredPosition.x <= -maxDistance && i%2 != 0)) 
                    Fail();

                var vector2 = playerImpulse[i].rectTransform.anchoredPosition;
                vector2.x += (i%2 == 0 ? speed : -speed);
                playerImpulse[i].rectTransform.anchoredPosition = vector2;
            }
        }

        public void StartDance(bool isDancing)
        {
            _currentIndex = 0;
            for (var i = 0; i < playerImpulse.Count; i++)
            {
                playerImpulse[i].gameObject.SetActive(true);
                notes[i].gameObject.SetActive(true);
            }
            _isDancing = isDancing;
            _isDestroyingMimic = !isDancing;
            if(isDancing)
                Dance(0);
            else
            {
                _currentMimicQTE = qteMimic[Random.Range(0, qteMimic.Count)];
                DanceMimic(0);
            }
        }

        public void SetInput(float tolerance)
        {
           if((notePositions[_currentIndex] > notes[0].rectTransform.anchoredPosition.x + tolerance) ||
              (notePositions[_currentIndex] < notes[0].rectTransform.anchoredPosition.x - tolerance))
           {
               Fail();
               
               if(_isDestroyingMimic)
                    print("alert");
           }
           else
           {
               if (_isDancing)
               {
                   _currentIndex += 1;
                   _currentIndex %= notePositions.Count;
                   Dance(_currentIndex);
               }
               else if (_isDestroyingMimic)
               {
                   DanceMimic(_currentIndex);
                   Fail();
               }
           }
        }
        
        private void Dance(int currentNote)
        {
            for (var i = 0; i < notes.Count; i++)
            {
                var vector3 = notes[i].rectTransform.anchoredPosition;
                vector3.x = i%2 == 0 ? notePositions[currentNote] : -notePositions[currentNote];
                notes[i].rectTransform.anchoredPosition = vector3;
                
                var playerVector3 = playerImpulse[i].rectTransform.anchoredPosition;
                playerVector3.x = 0;
                playerImpulse[i].rectTransform.anchoredPosition = playerVector3;
            }
        }
        
        
        private void DanceMimic(int currentNote)
        {
            for (var i = 0; i < notes.Count; i++)
            {
                var vector3 = notes[i].rectTransform.anchoredPosition;
                vector3.x = i%2 == 0 ? _currentMimicQTE.notes[currentNote] : - _currentMimicQTE.notes[currentNote];
                notes[i].rectTransform.anchoredPosition = vector3;
                
                var playerVector3 = playerImpulse[i].rectTransform.anchoredPosition;
                playerVector3.x = 0;
                playerImpulse[i].rectTransform.anchoredPosition = playerVector3;
            }
            if(_currentIndex == _currentMimicQTE.notes.Count)
                Fail();
        }

        
      
        private void Fail()
        {
            _isDancing = false;
            _isDestroyingMimic = false;
            PlayerRoot.SetIsDancing(false);
            for (var i = 0; i < playerImpulse.Count; i++)
            {
                playerImpulse[i].gameObject.SetActive(false);
                notes[i].gameObject.SetActive(false);
            }
        }
        
        [Serializable]
        private struct MimicDestructionQte
        {
            public List<int> notes;
        }
    }
}

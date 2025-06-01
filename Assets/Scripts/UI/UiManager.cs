using System;
using DG.Tweening;
using Game;
using Interactables;
using Player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Image inputShow;
        [SerializeField] private Image pausePanel;
        [SerializeField] private DancePanel dancePanel;
        [SerializeField] private TMP_Text usageText;
        [SerializeField] private Image mannequinMask;
        
        
        private Interactable _current;
        
        private static UiManager _instance;
        
        private void Awake()
        {
            if (_instance == null) 
                _instance = this;
            else 
                Destroy(this);

            if (usageText)
                usageText.text = "";
        }

        public static void SetInteract(Interactable interactable)
        {
            if(!_instance) 
                return;
            
            _instance._current = interactable;

            if (!_instance.usageText)
                return;

            if (!interactable)
            {
                _instance.usageText.text = "";
                return;
            }
            
            _instance.usageText.text = interactable.GetInteractionType() switch
            {
                InteractionType.Interactable => "Interact - E",
                InteractionType.GrabObject => "Grab - E\\nDestroy - A",
                InteractionType.Collectible => "Collect - E",
                InteractionType.Mannequin => "Enter - E",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static void SetGrab()
        {
            if(_instance && _instance.usageText) 
                _instance.usageText.text = "Drop - E\\nThrow - Left Click";
        }

        public static void SetDance(float tolerance)
        {
            var isDancing = PlayerRoot.GetIsDancing();
            _instance.inputShow.color = isDancing ? Color.blue : Color.white;
            _instance.inputShow.transform.DOScale(isDancing ? 0: 1, isDancing ? 1 : 0);
            
            if (!_instance.dancePanel) 
                return;
            
            if(isDancing)
                _instance.dancePanel.SetInput(tolerance);
            else 
                _instance.dancePanel.StartDance();
        }

        public static void InMannequin(bool isInMannequin = true)
        {
            if(_instance && _instance.mannequinMask)
                _instance.mannequinMask.gameObject.SetActive(isInMannequin);
            if( _instance.usageText && isInMannequin) 
                _instance.usageText.text = "Exit - E";
        }

        public static void PauseGame(bool setPause)
        {
            if(_instance &&  _instance.pausePanel)
                _instance.pausePanel.gameObject.SetActive(setPause);
            
            Time.timeScale = setPause ? 0f : 1f;
        }
        
    }
}

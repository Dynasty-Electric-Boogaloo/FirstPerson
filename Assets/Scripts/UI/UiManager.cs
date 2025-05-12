using System;
using DG.Tweening;
using Game;
using Interactables;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Image inputShow;
        [SerializeField] private TMP_Text usageText;
        
        
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
                InteractionType.GrabObject => "Grab - E\\nDestroy - A",
                InteractionType.Collectible => "Collect - E",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static void SetGrab()
        {
            _instance.usageText.text = "Drop - E\\nThrow - Left Click";
        }

        public static void SetDance(bool isDancing)
        {
            _instance.inputShow.color = isDancing ? Color.blue : Color.white;
            _instance.inputShow.transform.DOScale(isDancing ? 0: 1, isDancing ? 1 : 0);
        }
        
        
    }
}

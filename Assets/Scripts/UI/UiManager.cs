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
        [SerializeField] private TMP_Text usageText;
        [SerializeField] private Image mannequinMask;
        
        private Interactable _current;
        
        private static UiManager _instance;
        
        private void Awake()
        {
            if (_instance == null) 
                _instance = this;
            if (usageText)
                usageText.text = "";
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
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
                InteractionType.GrabObject => "Grab - E",
                InteractionType.Collectible => "Collect - E",
                InteractionType.Mannequin => "Enter - E",
                _ => throw new ArgumentOutOfRangeException()
            };

            if (interactable.gameObject.GetComponent<Inspectable>())
            {
                _instance.usageText.text += "\\nInspect - I";
            }
        }

        public static void SetGrab()
        {
            if(_instance && _instance.usageText) 
                _instance.usageText.text = "Drop - E\\nThrow - Left Click";
        }
        
        public static void InMannequin(bool isInMannequin = true)
        {
            if(!_instance)
                return;
            
            if( _instance.mannequinMask)
                _instance.mannequinMask.gameObject.SetActive(isInMannequin);
            if( _instance.usageText && isInMannequin) 
                _instance.usageText.text = "Exit - E";
        }

        public static void SetInspect()
        {
            if(!_instance) 
                return;
            
            _instance.usageText.text = "Return - I";
        }
    }
}

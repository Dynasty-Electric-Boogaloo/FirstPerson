using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Image inputShow;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite grabbingSprite;
        
        private Interactable _current;
        
        private static UiManager _instance;
        
        private void Awake()
        {
            if (_instance == null) 
                _instance = this;
            else 
                Destroy(this);
        }

        public static void CanInteract(Interactable interactable)
        {
            if(!interactable || !_instance) 
                return;
            
            _instance._current = interactable;
        }
    }
}

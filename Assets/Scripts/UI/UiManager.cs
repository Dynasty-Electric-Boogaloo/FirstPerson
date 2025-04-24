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
        
        private static UiManager _userInterface;
        
        private void Awake()
        {
            if (_userInterface == null) 
                _userInterface = this;
            else 
                Destroy(this);
        }
        
        public void CanInteract(Interactable interactable)
        {
            if(interactable == null) 
                return;
            
            _current = interactable;
        }
    }
}

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
        
        public static UiManager UserInterface;
        private void Awake()
        {
            if (UserInterface == null) UserInterface = this;
            else Destroy(this);
        }
        public void CanInteract(bool canInteract, Interactable interactable)
        {
            _current = interactable;
        }
    }
}

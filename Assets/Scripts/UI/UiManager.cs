using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Image inputShow;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite grabbingSprite;
        [SerializeField] private TMP_Text usageText;
        
        private Interactable _current;
        
        private static UiManager _instance;
        
        private void Awake()
        {
            if (_instance == null) 
                _instance = this;
            else 
                Destroy(this);
        }

        public static void SetInteract(Interactable interactable)
        {
            if(!_instance) 
                return;
            _instance.usageText.text = interactable != null ? "Grab - E\\nDestroy - A" : "";
            _instance._current = interactable;
        }
    }
}

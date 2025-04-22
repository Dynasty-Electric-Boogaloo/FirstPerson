using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image inputShow;
        private Dictionary<IInteractable, Image> _elements;
        
        public static UiManager UserInterface;
        private void Awake()
        {
            if (UserInterface == null) UserInterface = this;
            else Destroy(this);
        }

        public void AddInput(IInteractable interactable)
        {
        }

        public void RemoveInput(IInteractable interactable)
        {
        }
        
    }
}

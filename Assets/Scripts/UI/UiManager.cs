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
        [SerializeField] private GameObject inputShow;
        private Dictionary<IInteractable, GameObject> _elements;
        
        public static UiManager UserInterface;
        private void Awake()
        {
            if (UserInterface == null) UserInterface = this;
            else Destroy(this);
        }

        public void AddInput(IInteractable interactable)
        {
            print(interactable.Transform.gameObject);
        }

        public void RemoveInput(IInteractable interactable)
        {
        }
        
    }
}

using System;
using Interactables;
using UnityEngine;
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
        //Code only for first playable, nuke it afterwards
        [SerializeField] private TMP_Text collectedText;
        
        private Interactable _current;
        
        private static UiManager _instance;
        
        //Code only for first playable, nuke it afterwards
        private int _collectibleCount;
        private int _collectedCount;
        
        private void Awake()
        {
            if (_instance == null) 
                _instance = this;
            else 
                Destroy(this);

            if (usageText)
                usageText.text = "";
            
            //Code only for first playable, nuke it afterwards
            _collectibleCount = FindObjectsByType<Collectible>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
            _collectedCount = 0;
            collectedText.text = $"{_instance._collectedCount}/{_instance._collectibleCount}";
        }
        
        //Code only for first playable, nuke it afterwards
        public static void AddCollected()
        {
            if (!_instance)
                return;
            
            _instance._collectedCount++;

            if (!_instance.collectedText)
                return;
            
            _instance.collectedText.text = $"{_instance._collectedCount}/{_instance._collectibleCount}";
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
    }
}

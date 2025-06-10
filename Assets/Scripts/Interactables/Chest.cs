using System;
using DG.Tweening;
using UnityEngine;

namespace Interactables
{
    public class Chest : Interactable
    {
        [SerializeField] private GameObject door;
        [SerializeField] private Transform openDoor;
        [SerializeField] private Transform closedDoor;
        [SerializeField] private float timeToOpen;
        [SerializeField] private bool baseIsOpen;
        private bool _isOpen;

        private void Start()
        {
            SetUp();
        }

        private void SetUp()
        {
            _isOpen = baseIsOpen;
            door.transform.DOLocalRotate(baseIsOpen ? openDoor.localRotation.eulerAngles : closedDoor.localRotation.eulerAngles, timeToOpen);
            door.transform.DOLocalMove(baseIsOpen ? openDoor.localPosition : closedDoor.localPosition, timeToOpen);
        }

        public override void Interact()
        {
            base.Interact();
            
            _isOpen = !_isOpen;
            
            door.transform.DOLocalRotate(_isOpen ? openDoor.localRotation.eulerAngles : closedDoor.localRotation.eulerAngles, timeToOpen);
            door.transform.DOLocalMove(_isOpen ? openDoor.localPosition : closedDoor.localPosition, timeToOpen);
        }
        
        public override void Restore()
        {
            base.Restore();

            SetUp();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public interface  IInteractable
    {
        void Highlight(bool canInteract);
        void Interact();

        Transform Transform {get ; set; }
    }
}

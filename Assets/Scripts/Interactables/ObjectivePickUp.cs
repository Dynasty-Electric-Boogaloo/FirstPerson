using Player;
using UnityEngine;

public class ObjectivePickUp : MonoBehaviour
{
    [SerializeField] private Door trap;
    public void PickedUp()
    {
        gameObject.SetActive(false);
        if(trap)
            trap.ChangeState();
    }
    
}

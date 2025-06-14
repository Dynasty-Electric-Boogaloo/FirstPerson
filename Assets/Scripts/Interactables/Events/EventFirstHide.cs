using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;

public class EventFirstHide : EventObject
{
    [SerializeField] private List<Door> doors;
    public override void DoEvent()
    {
        base.DoEvent();
        
        StartCoroutine(Event());
    }

    private IEnumerator Event()
    {
        PlayerRoot.SetIsLocked(true);
        yield return new WaitForSeconds(2.5f);
        UiManager.SetChaseBorder(false);
        yield return new WaitForSeconds(2.5f);
        PlayerRoot.SetIsLocked(false);
        UiManager.InMannequin();
        foreach (var door in doors)
            door.ChangeState(true);
        enabled = false;
    }
}

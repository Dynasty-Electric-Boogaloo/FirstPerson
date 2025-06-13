using System.Collections.Generic;
using Player;
using UnityEngine;

public class EventRedLight : EventObject
{
    [SerializeField] private List<Door> doors;
    public override void DoEvent()
    {
        base.DoEvent();
        PlayerRoot.SetRedLight(true);
        
        foreach (var door in doors)
            door.ChangeState(true);
        
        InformationManager.SetText("Press [F] to switch light", 2);
        
        //ça sera a nuke
        Invoke(nameof(SetTextObjectif), 2.25f);
    }

    private void SetTextObjectif()
    {
        InformationManager.SetText("Find the missing parts of your music box ([C])", 2);
    }
}

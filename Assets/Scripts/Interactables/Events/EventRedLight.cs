using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UI;
using UnityEngine;

public class EventRedLight : EventObject
{
    [SerializeField] private Renderer wallClue;
    public override void DoEvent()
    {
        base.DoEvent();
        PlayerRoot.SetRedLight(true);
        
        InformationManager.SetText("Press [F] to switch light", 2);
        StartCoroutine(Event());
    }

    private IEnumerator Event()
    {
        yield return new WaitForSeconds(2.5f);
        UiManager.SetChaseBorder(true);
        //bruitFakeChase
        yield return new WaitForSeconds(1f);
        //Son tapper la porte
        yield return new WaitForSeconds(1f);
        
        if(wallClue)
            wallClue.gameObject.SetActive(true);
    }

    private void Start()
    {
        if(wallClue)
            wallClue.gameObject.SetActive(false);
    }
}

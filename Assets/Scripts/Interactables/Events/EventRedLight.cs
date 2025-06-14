using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UI;
using UnityEngine;

public class EventRedLight : EventObject
{
    [SerializeField] private SpriteRenderer wallClue;
    private MeshRenderer mesh;
    private BoxCollider box;
    
    public override void DoEvent()
    {
        base.DoEvent();
        
        if (mesh)
            mesh.enabled = false;
        
        if(box)
            box.enabled = false;
        
        UiManager.SetEmpty();
        
        PlayerRoot.SetRedLight(true);
        
        InformationManager.SetText("Press [F] to switch light", 2);
        StartCoroutine(Event());
    }

    private IEnumerator Event()
    {
        
        yield return new WaitForSeconds(2.5f);
        UiManager.SetChaseBorder(true);
        //bruitFakeChase
        yield return new WaitForSeconds(0.5f);
        //Son tapper la porte
        
        if(wallClue)
        {
            wallClue.gameObject.SetActive(true);
            wallClue.DOFade(1, 1);
        }
        
        yield return new WaitForSeconds(1f);
        
        gameObject.SetActive(false);
    }

    private void Start()
    {
        if (!wallClue) 
            return;
        
        wallClue.gameObject.SetActive(false);
        wallClue.DOFade(0, 0);
        
        if (GetComponentInChildren<MeshRenderer>())
           mesh = GetComponent<MeshRenderer>();
        
        if (TryGetComponent<BoxCollider>(out var col))
            box = col;
    }
}

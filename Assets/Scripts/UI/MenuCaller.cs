using System;
using Monster;
using Player;
using UnityEngine;

public class MenuCaller : MonoBehaviour
{
    [SerializeField] Camera menuCamera;
    [SerializeField] Animator anim;

    private void Start()
    {
        PlayerRoot.SetVisible(false);
        MonsterRoot.SetVisible(false);
    }
    
    public void Menu()
    {
        //anim.Play("Turn");
        MenuManager.LoadWorld();
        menuCamera.enabled = false;
        PlayerRoot.SetVisible(true);
        MonsterRoot.SetVisible(true);
        
    }
    
    public void Quit()
    {
       Application.Quit();
    }

    public void ChangeSize()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}

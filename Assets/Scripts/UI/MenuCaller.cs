using System;
using Monster;
using Player;
using UnityEngine;

public class MenuCaller : MonoBehaviour
{
    [SerializeField] Camera menuCamera;

    private void Start()
    {
        PlayerRoot.SetVisible(false);
        MonsterRoot.SetVisible(false);
    }
    
    public void Menu()
    {
        MenuManager.LoadWorld();
        menuCamera.enabled = false;
        PlayerRoot.SetVisible(true);
        MonsterRoot.SetVisible(true);
    }
    
    public void Quit()
    {
       Application.Quit();
    }
}

using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;
    [SerializeField] private int menuGroup;
    [SerializeField] private int dressingRoomGroup;
    [SerializeField] private List<int> worldGroups;

    private void Start()
    {
        if (_instance == null)
            _instance = this;
        
        SceneLoader.LoadSceneGroup(menuGroup, LoadSceneMode.Additive);
        SceneLoader.LoadSceneGroup(dressingRoomGroup, LoadSceneMode.Additive);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    public static void LoadWorld()
    {
        if (_instance == null)
            return;
        
        SceneLoader.UnloadSceneGroupAsync(_instance.menuGroup);
        
        foreach (var sceneGroup in _instance.worldGroups)
        {
            SceneLoader.LoadSceneGroup(sceneGroup, LoadSceneMode.Additive);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
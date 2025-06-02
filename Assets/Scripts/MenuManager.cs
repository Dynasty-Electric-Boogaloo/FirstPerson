using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private int menuGroup;
    [SerializeField] private int dressingRoomGroup;
    [SerializeField] private List<int> worldGroups;

    private void Start()
    {
        SceneLoader.LoadSceneGroup(menuGroup, LoadSceneMode.Additive);
        SceneLoader.LoadSceneGroup(dressingRoomGroup, LoadSceneMode.Additive);
    }

    public void LoadWorld()
    {
        SceneLoader.UnloadSceneGroupAsync(menuGroup);
        
        foreach (var sceneGroup in worldGroups)
        {
            SceneLoader.LoadSceneGroup(sceneGroup, LoadSceneMode.Additive);
        }
    }
}
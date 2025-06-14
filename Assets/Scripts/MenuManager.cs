using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;
    [SerializeField] private int menuGroup;
    [SerializeField] private int dressingRoomGroup;
    [SerializeField] private List<int> worldGroups;
    private List<Task> _loadingTasks;

    private void Start()
    {
        if (_instance == null)
            _instance = this;
        
        SceneLoader.LoadSceneGroup(menuGroup, LoadSceneMode.Additive);
        SceneLoader.LoadSceneGroup(dressingRoomGroup, LoadSceneMode.Additive);
        _loadingTasks = new List<Task>();

        foreach (var group in worldGroups)
        {
            _loadingTasks.Add(SceneLoader.LoadSceneGroupAsync(group, LoadSceneMode.Additive));
        }
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
        
        _instance._loadingTasks.Add(SceneLoader.UnloadSceneGroupAsync(_instance.menuGroup));
        
        /*foreach (var sceneGroup in _instance.worldGroups)
        {
            SceneLoader.LoadSceneGroup(sceneGroup, LoadSceneMode.Additive);
        }*/

        Task.WhenAll(_instance._loadingTasks);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
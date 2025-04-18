using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldLoader : MonoBehaviour
{
    [SerializeField] private List<int> sceneGroups;

    private void Start()
    {
        Load();
    }

    private void Load()
    {
        foreach (var sceneGroup in sceneGroups)
        {
            SceneLoader.LoadSceneGroupAsync(sceneGroup, LoadSceneMode.Additive);
        }
    }
}
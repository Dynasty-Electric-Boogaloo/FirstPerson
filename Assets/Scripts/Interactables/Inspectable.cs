using System;
using System.Collections.Generic;
using Interactables;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Inspectable : MonoBehaviour
{
    [SerializeField] private List<string> possible = new List<string>();
    [SerializeField] private int index;

    public void Inspect()
    {
        InspectSystem.Show(index, possible.Count > 0 ? possible[Random.Range(0, possible.Count)] : "");
        PauseManager.PauseGame(true, false);
    }
}

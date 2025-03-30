using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new QTE", menuName = "Scriptable Objects/Create New QTE")]
public class ScriptableQte : ScriptableObject
{
    public List<KeyCode> possibleInput = new();
    public float minimumInputs;
    public float maximumInputs;
    public float minimumTimeByInput;
    public float maximumTimeByInput;
    
}

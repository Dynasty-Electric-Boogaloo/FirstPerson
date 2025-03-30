using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "new QTE", menuName = "Scriptable Objects/Create New QTE")]
public class ScriptableQte : ScriptableObject
{
    public InputActionMap possibleInput = new();
    public float minimumInputs;
    public float maximumInputs;
    public float minimumTimeByInput;
    public float maximumTimeByInput;
    
}

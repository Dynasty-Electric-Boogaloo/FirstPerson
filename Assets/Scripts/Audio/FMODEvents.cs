using System;
using UnityEngine;
using FMODUnity;
using EventReference = FMODUnity.EventReference;

public class FMODEvents : MonoBehaviour
{
    
    [Header("Ambiant")]
    [field : SerializeField] public EventReference Piano { get; private set;}
    [field : SerializeField] public EventReference Whispers { get; private set;}
    [field : SerializeField] public EventReference Flicker { get; private set;}
    [field : SerializeField] public static EventReference Step { get; private set;}

    //public static EventReference GetStep() => instance ? instance.Step : new EventReference();
    public static EventReference GetFlicker() => instance ? instance.Flicker : new EventReference();

    public static FMODEvents instance {get; private set;}
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in this scene.");
            return;
        }
        instance = this;
    }
    
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}

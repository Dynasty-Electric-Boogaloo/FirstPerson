using System;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;
using FMODUnity;
using EventReference = FMODUnity.EventReference;

public class FMODEvents : MonoBehaviour
{
    
    [field: Header("Ambiant")]
    [field: SerializeField] public EventReference Phonographe { get; private set;}
    [field: SerializeField] public EventReference Whispers { get; private set;}
    
    
    public static FMODEvents instance {get; private set;}
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in this scene.");
        }
        instance = this;
    }
}

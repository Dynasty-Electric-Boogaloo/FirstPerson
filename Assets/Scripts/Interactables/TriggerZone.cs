using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TriggerZone : MonoBehaviour
{
    public UnityEvent setOnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        setOnTrigger.Invoke();
    }
}

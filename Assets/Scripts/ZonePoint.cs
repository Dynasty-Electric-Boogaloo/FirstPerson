using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways, Serializable]
public class ZonePoint : MonoBehaviour
{
    [SerializeField] private int roomId;

#if UNITY_EDITOR
    private Vector3 _lastPosition;
            
    private void Update()
    {
        if (EditorApplication.isPlaying)
            return;

        if (transform.position == _lastPosition)
            return;
            
        _lastPosition = transform.position;
    }
#endif
}
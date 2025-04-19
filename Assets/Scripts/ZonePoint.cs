using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways, Serializable]
public class ZonePoint : MonoBehaviour
{
    [SerializeField] private int roomId;

#if UNITY_EDITOR
    private Vector3 _lastPosition;

    private void OnEnable()
    {
        if (EditorApplication.isPlaying || ZoneGraphManager.Instance == null)
            return;
        
        ZoneGraphManager.Instance.ComputeZones();
    }

    private void OnDisable()
    {
        if (EditorApplication.isPlaying || ZoneGraphManager.Instance == null)
            return;

        ZoneGraphManager.Instance.ComputeZones();
    }

    private void Update()
    {
        if (EditorApplication.isPlaying || ZoneGraphManager.Instance == null)
            return;

        if (transform.position == _lastPosition)
            return;
        
        ZoneGraphManager.Instance.ComputeZones();
        _lastPosition = transform.position;
    }
#endif
    
    public int GetRoom()
    {
        return roomId;
    }

    public void SetRoom(int id)
    {
        roomId = id;
    }
}
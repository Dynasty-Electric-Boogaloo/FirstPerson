using System;
using System.Collections.Generic;
using UnityEngine;

public class ZoneGraphManager : MonoBehaviour
{
    [Serializable]
    private struct Node
    {
        public ZonePoint point;
        public int[] 
    }
    
    private static ZoneGraphManager _instance;
    private List<ZonePoint> _zonePoints;
    private Dictionary<int, List<int>> _rooms;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    public static ZonePoint GetPointByIndex(int index)
    {
        if (_instance == null)
            return null;

        if (index < 0 || index >= _instance._zonePoints.Count)
            return null;
        
        return _instance._zonePoints[index];
    }

    public void ComputeZones()
    {
        var zonePoints = FindObjectsByType<ZonePoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        
    }
}
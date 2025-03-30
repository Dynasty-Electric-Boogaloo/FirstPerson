using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ZoneGraphManager))]
public class ZoneGraphManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Compute graph"))
        {
            ((ZoneGraphManager)target).ComputeZones();
        }
    }
}
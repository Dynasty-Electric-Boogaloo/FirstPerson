using UnityEditor;
using UnityEngine;
using ZoneGraph;

[CustomEditor(typeof(ZoneGraphBuilder))]
public class ZoneGraphComputerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Compute graph"))
        {
            ((ZoneGraphBuilder)target).ComputeZones();
            SceneView.RepaintAll();
        }
    }
}
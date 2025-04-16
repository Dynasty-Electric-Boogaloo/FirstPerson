using System;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneLoaderConfig))]
public class SceneLoaderConfigEditor : Editor
{
    SceneLoaderConfig t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;

    private void OnEnable()
    {
        t = (SceneLoaderConfig)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("sceneReferences");
    }

    public override void OnInspectorGUI()
    {
        GetTarget.Update();
        
        GUILayout.Label("Scene References");
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        ListSize = ThisList.arraySize;
        ListSize = EditorGUILayout.IntField("Size", ListSize);
    }
}
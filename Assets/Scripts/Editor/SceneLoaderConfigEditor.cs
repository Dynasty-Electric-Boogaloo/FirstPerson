using System;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneLoaderConfig))]
public class SceneLoaderConfigEditor : Editor
{
    private SceneLoaderConfig _t;
    private SerializedObject _getTarget;
    private SerializedProperty _thisList;
    private int _listSize;
    private bool[] _folded;

    private SceneAsset _refScene;

    private void OnEnable()
    {
        _t = (SceneLoaderConfig)target;
        _getTarget = new SerializedObject(_t);
        _thisList = _getTarget.FindProperty("sceneReferences");
        _folded = new bool[_t.sceneReferences.Length];
    }

    public override void OnInspectorGUI()
    {
        _getTarget.Update();
        Show(_thisList);
        _getTarget.ApplyModifiedProperties();
    }
    
    public void Show(SerializedProperty list)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(list, false);
        var listSize = EditorGUILayout.DelayedIntField(list.arraySize);
        
        if (listSize != list.arraySize)
        {
            list.arraySize = listSize;
            Array.Resize(ref _folded, listSize);
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel++;

        if (list.isExpanded)
        {
            for (var i = 0; i < list.arraySize; i++)
            {
                _folded[i] = EditorGUILayout.BeginFoldoutHeaderGroup(_folded[i], $"Scene Group {i}");
                
                if (!_folded[i])
                {
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    continue;
                }

                var sceneLabelContent = new GUIContent("Decoration scene");
                var labelStyle = new GUIStyle(GUI.skin.label);
                var width = labelStyle.CalcSize(sceneLabelContent).x;
                
                _t.sceneReferences[i].logicSceneGuid = SceneReferenceField("Logic scene", _t.sceneReferences[i].logicSceneGuid, width);
                _t.sceneReferences[i].blockoutSceneGuid = SceneReferenceField("Blockout scene", _t.sceneReferences[i].blockoutSceneGuid, width);
                _t.sceneReferences[i].decoSceneGuid = SceneReferenceField("Decoration scene", _t.sceneReferences[i].decoSceneGuid, width);
                
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        
        _t.ComputeBuildList();

        EditorGUI.indentLevel--;
    }

    private string SceneReferenceField(string label, string guid, float labelWidth)
    {
        EditorGUILayout.BeginHorizontal();
        var labelStyle = new GUIStyle(GUI.skin.label)
        {
            fixedWidth = labelWidth
        };
        
        GUILayout.Label(label, labelStyle);
                
        _refScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(guid));
        _refScene = (SceneAsset)EditorGUILayout.ObjectField(_refScene, typeof(SceneAsset), false);
        
        EditorGUILayout.EndHorizontal();
        
        return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_refScene));
    }
}
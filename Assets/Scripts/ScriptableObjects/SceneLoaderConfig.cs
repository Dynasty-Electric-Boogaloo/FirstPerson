using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Scene Loader Config", fileName = "Scene Loader Config")]
    public class SceneLoaderConfig : ScriptableObject
    {
        [Serializable]
        public struct SceneReference
        {
            public string logicSceneGuid;
            public string blockoutSceneGuid;
            public string decoSceneGuid;
        }

        [Serializable]
        public struct SceneIds
        {
            public int[] ids;
        }

        public SceneReference[] sceneReferences;
        public SceneIds[] sceneIds;

        public void ComputeBuildList()
        {
            var editorBuildScenes = new List<EditorBuildSettingsScene>();
            sceneIds = new SceneIds[sceneReferences.Length];

            for (var i = 0; i < sceneReferences.Length; i++)
            {
                sceneIds[i].ids = new int[3];
                for (var j = 0; j < 3; j++)
                {
                    sceneIds[i].ids[j] = -1;
                }

                var currentId = 0;
                
                var sceneGuid = sceneReferences[i].logicSceneGuid;
                if (!string.IsNullOrEmpty(sceneGuid))
                {
                    sceneIds[i].ids[currentId++] = editorBuildScenes.Count;
                    editorBuildScenes.Add(new EditorBuildSettingsScene(new GUID(sceneGuid), true));
                }
                
                sceneGuid = sceneReferences[i].blockoutSceneGuid;
                if (!string.IsNullOrEmpty(sceneGuid))
                {
                    sceneIds[i].ids[currentId++] = editorBuildScenes.Count;
                    editorBuildScenes.Add(new EditorBuildSettingsScene(new GUID(sceneGuid), true));
                }
                
                sceneGuid = sceneReferences[i].decoSceneGuid;
                if (!string.IsNullOrEmpty(sceneGuid))
                {
                    sceneIds[i].ids[currentId] = editorBuildScenes.Count;
                    editorBuildScenes.Add(new EditorBuildSettingsScene(new GUID(sceneGuid), true));
                }
            }

            EditorBuildSettings.scenes = editorBuildScenes.ToArray();
        }
    }
}
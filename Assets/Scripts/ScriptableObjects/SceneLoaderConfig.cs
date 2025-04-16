using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Scene Loader Config", fileName = "Scene Loader Config")]
    public class SceneLoaderConfig : ScriptableObject
    {
        [Serializable]
        public struct SceneReference
        {
            public GUID logicSceneGuid;
            public GUID blockoutSceneGuid;
            public GUID decoSceneGuid;
            public bool included;
        }

        public SceneReference[] sceneReferences;

        private void OnValidate()
        {
            var editorBuildScenes = new List<EditorBuildSettingsScene>();

            foreach (var sceneReference in sceneReferences)
            {
                if (!sceneReference.included)
                    continue;

                var sceneGuid = sceneReference.logicSceneGuid;
                if (!sceneGuid.Empty())
                {
                    editorBuildScenes.Add(new EditorBuildSettingsScene(sceneGuid, true));
                }
                
                sceneGuid = sceneReference.blockoutSceneGuid;
                if (!sceneGuid.Empty())
                {
                    editorBuildScenes.Add(new EditorBuildSettingsScene(sceneGuid, true));
                }
                
                sceneGuid = sceneReference.decoSceneGuid;
                if (!sceneGuid.Empty())
                {
                    editorBuildScenes.Add(new EditorBuildSettingsScene(sceneGuid, true));
                }
            }

            EditorBuildSettings.scenes = editorBuildScenes.ToArray();
        }
    }
}
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class SceneDataManager : IPreprocessBuildWithReport
{
    [MenuItem("Tools/Test scene detection")]
    private static void Test()
    {
        var assets = AssetDatabase.FindAssets("t:SceneAsset");
        foreach (var asset in assets)
        {
            Debug.Log(Path.GetFileName(AssetDatabase.GUIDToAssetPath(asset)));
        }
    }

    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildReport report)
    {
        //report.scenesUsingAssets
    }
}
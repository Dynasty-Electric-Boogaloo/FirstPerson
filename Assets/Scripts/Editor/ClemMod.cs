using System;
using UnityEditor;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ClemMod : MonoBehaviour
{
    //En public pour pouvoir etre utilisé par d'autres scripts au cas où
    public static bool lightMod;
    
    [MenuItem("Tools/Clem Mod")]
    static void ChangeModSettings()
    {
        Debug.Log("Changing Mod Settings...");
        lightMod = !lightMod; 
        Debug.Log("Mod is now " + lightMod);
    }
    
    /// <summary>
    /// Check si Clem est l'user et si oui passe directement en clem mod 
    /// </summary>
    [InitializeOnLoad]
    static class CheckingUser
    {
        static CheckingUser()
        {
            if (Environment.UserName == "c.lai") lightMod = true;
        }
    }
}


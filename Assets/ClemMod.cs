using System;
using UnityEditor;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;

public class ClemMod : MonoBehaviour
{

    [MenuItem("Tools/Clem Mod")]
    static void ChangeModSettings()
    {
        Debug.Log("Changing Mod Settings...");
    }

    [InitializeOnLoad]
    static class CheckingUser
    {
        static CheckingUser()
        {
            if (Environment.UserName == "p.farin") ChangeModSettings();
        }
    }
}
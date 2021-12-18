using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GenScenePath))]
public class GenScenePathEditor : Editor
{

    public override void OnInspectorGUI()
    {
        GenScenePath pre = (GenScenePath)target;

        if (GUILayout.Button("Create Path"))
        {
            pre.CreatePoly();
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGeneration))]
public class MapGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGeneration script = (MapGeneration)target;

        if (GUILayout.Button("Regenerate Path", GUILayout.Height(30)))
        {
            script.RegeneratePath();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawDefaultInspector();

    }
}

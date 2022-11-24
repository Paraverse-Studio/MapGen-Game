#if UNITY_EDITOR
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

        if (GUILayout.Button("Regenerate Map", GUILayout.Height(30)))
        {
            script.RegenerateMap();
        }

        //if (GUILayout.Button("Render Blocks", GUILayout.Height(30)))
        //{
        //    script.RenderBlocks();
        //}

        for (int i = 0; i < 3; ++i)
        {
            EditorGUILayout.Space();
        }
        
        DrawDefaultInspector();

    }
}
#endif
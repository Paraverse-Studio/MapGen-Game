#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;

[CustomEditor(typeof(NavMeshBuilder))]
public class TestingNavMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NavMeshBuilder script = (NavMeshBuilder)target;

        if (GUILayout.Button("Build NavMesh", GUILayout.Height(30)))
        {
            script.BuildNavMesh();
        }
        if (GUILayout.Button("Update NavMesh", GUILayout.Height(30)))
        {
            script.UpdateNavMesh();
        }
        for (int i = 0; i < 3; ++i)
        {
            EditorGUILayout.Space();
        }

        DrawDefaultInspector();

    }
}
#endif
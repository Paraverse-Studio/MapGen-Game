#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;

[CustomEditor(typeof(TestingNavMesh))]
public class TestingNavMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TestingNavMesh script = (TestingNavMesh)target;

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
using UnityEngine;
using System.Collections;
using UnityEditor;
using XftWeapon;

[CustomEditor(typeof(XWeaponTrail))]
[CanEditMultipleObjects]
public class XWeaponTrailEditor : Editor
{

    //Texture2D _icon;


    void OnEnable() {
        //_icon = Resources.Load("x-weapontrail_logo") as Texture2D;
    }

    public override void OnInspectorGUI()
    {


        EditorGUILayout.BeginVertical();

        //if (GUILayout.Button(_icon, new GUIStyle())) {
        //    Application.OpenURL("");
        //}

        EditorGUILayout.LabelField("Version: " + XWeaponTrail.Version);
        EditorGUILayout.LabelField("Email: shallwaycn@gmail.com");
        //EditorGUILayout.LabelField("Web: http://phantomparticle.org");
        EditorGUILayout.EndVertical();

        GUI.color = Color.green;
        if (GUILayout.Button("Documentation", GUILayout.Height(32))) {
            Application.OpenURL("http://shallway.ai/x-weapontrail/documentation/");
        }
		if (GUILayout.Button("Unity Forum", GUILayout.Height(32))) {
			Application.OpenURL("https://forum.unity.com/threads/x-weapontrail-the-best-weapon-trail-solution-in-unity.262954");
		}
		GUI.color = Color.white;

        GUILayout.Space(10);
        //DrawDefaultInspector();

        SerializedProperty UseWith2D = serializedObject.FindProperty("UseWith2D");
        EditorGUILayout.PropertyField(UseWith2D); 
        if (UseWith2D.boolValue) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SortingLayerName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SortingOrder"));
        }

        SerializedProperty UseWithSRP = serializedObject.FindProperty("UseWithSRP");
        EditorGUILayout.PropertyField(UseWithSRP); 

        EditorGUILayout.PropertyField(serializedObject.FindProperty("PointStart"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PointEnd"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxFrame"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Granularity"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Fps"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MyColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MyMaterial"));

        if (GUI.changed) {

            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(target);
        }
    }
}


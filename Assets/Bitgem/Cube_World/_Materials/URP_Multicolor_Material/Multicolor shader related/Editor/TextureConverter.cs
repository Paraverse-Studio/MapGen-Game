using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
public class TextureConverter : EditorWindow
{
    [MenuItem("Tools/Texture converter")]
    private static void ShowWindow()
    {
        GetWindow<TextureConverter>("Texture converter");
    }

    [SerializeField]
    private Texture2D highlights;

    [SerializeField]
    private Texture2D shadows;

    [SerializeField]
    private Texture2D metallic;

    [SerializeField]
    private Texture2D glow;

    [SerializeField]
    private string convertedFormat = "{0}_shadows_and_highlights.{1}";

    private bool ChangeReadability(Texture2D texture, bool state)
    {
        var assetPath = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
        var wasReadable = importer.isReadable;
        importer.isReadable = state;
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        return wasReadable;
    }

    private void OnGUI()
    {
        convertedFormat = EditorGUILayout.TextField(new GUIContent("Converted file name format"), convertedFormat);

        highlights = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Highlights"), highlights, typeof(Texture2D), false);
        shadows = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Shadows"), shadows, typeof(Texture2D), false);
        metallic = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Metallic"), metallic, typeof(Texture2D), false);
        glow = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Glow"), glow, typeof(Texture2D), false);
        if (highlights == null || shadows == null || metallic == null)
            return;

        if (!GUILayout.Button("Convert"))
            return;

        var wasShadowsReadable = ChangeReadability(shadows, true);
        var wasHighlightsReadable = ChangeReadability(highlights, true);
        var wasMetallicReadable = ChangeReadability(metallic, true);
        var glowWasReadable = glow == null ? false : ChangeReadability(glow, true);

        var result = new Texture2D(highlights.width, highlights.height, TextureFormat.ARGB32, true);
        
        for (var x = 0; x < highlights.width; x++)
        {
            for (var y = 0; y < highlights.height; y++)
            {
                var metallicPixel = metallic.GetPixel(x, y);
                var glowAmount = 0.0f;
                if (glow != null)
                    glowAmount = glow.GetPixel(x, y).maxColorComponent > 0.0f ? 1.0f : 0.0f;

                var color = new Color(highlights.GetPixel(x, y).a, shadows.GetPixel(x, y).a, metallicPixel.r, glowAmount);
                result.SetPixel(x, y, color);
            }
        }

        result.Apply(true);

        ChangeReadability(shadows, wasShadowsReadable);
        ChangeReadability(highlights, wasHighlightsReadable);
        ChangeReadability(metallic, wasMetallicReadable);
        if (glow != null)
            ChangeReadability(glow, glowWasReadable);

        var assetPath = AssetDatabase.GetAssetPath(highlights);
        var extention = Path.GetExtension(assetPath);
        var noExtentionPath = string.IsNullOrEmpty(extention) ? assetPath : assetPath.Substring(0, assetPath.Length - extention.Length);

        var convertedPath = string.Format(convertedFormat, noExtentionPath,
            string.IsNullOrEmpty(extention) ?
            string.Empty :
            "png");

        File.WriteAllBytes(convertedPath, result.EncodeToPNG());
        AssetDatabase.Refresh();
    }
}
#endif
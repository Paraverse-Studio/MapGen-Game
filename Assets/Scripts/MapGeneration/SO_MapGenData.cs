using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "SO_MapGenData", menuName = "SOs/MapGen/SO_MapGenData")]
public class SO_MapGenData : ScriptableObject
{
    [Space(30)]
    [Header("    _____________  MAP BASE  _____________")]
    [Space(5)]
    [MinMaxSlider(-1f, 1f)]
    public Vector2 randomElevation;

    [Header("        PATH SIZE ")]
    public float distanceOfPath = 40f;

    [Header("        PATH TWISTING ")]
    [MinMaxSlider(0f, 30f)]
    public Vector2 distanceBeforeTurningPath;

    [MinMaxSlider(0f, 100f)]
    public Vector2 turningAngleRange;

    [Header("        PATH THICKNESS ")]
    public int pathThickenFrequency = 8;

    [MinMaxSlider(0f, 50f)]
    public Vector2 grassFillRadius;

    [Header("The higher, the more square-ish")]
    [Range(1.0f, 1.5f)]
    public float circularity = 1.0f;

    [Header("        LUMPS ")]
    [Space(10)]
    [Range(0, 40)]
    public int lumpDensity;

    public int lumpApplicationRounds = 3;

    [MinMaxSlider(0f, 100f)]
    public Vector2 lumpRadius;

    public int lumpOffset = 2;

    [Header("        DIRT PATH ")]
    [Space(10)]
    public int dirtPathThickenFrequency = 8;

    [MinMaxSlider(0f, 40f)]
    public Vector2 dirtFillRadius;

    public int dirtCutoffFrequency;

    [MinMaxSlider(0f, 12f)]
    public Vector2 dirtCutoffLength;

    [Space(30)]
    [Header("    _____________  MAP PROPS  _____________")]
    [Space(30)]
    public bool showProps;

    [Header("        TREE SPAWNING ")]
    [Range(0, 10)]
    public int treeSpawnDensity = 4;
    public int treeSpawnOffset = 2;
    public float treeChanceGrowthRate = 5.0f;
}
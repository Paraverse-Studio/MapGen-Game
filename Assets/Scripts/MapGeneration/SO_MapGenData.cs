using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;

/*
 * BlockSet are predetermined set of block types needed to have a valid map,
 * such as a grass item, dirt item, etc. 
 */
[System.Serializable]
public struct BlockSet
{
    public SO_BlockItem grass;
    public SO_BlockItem dirt;
    public SO_BlockItem water;
    public SO_BlockItem foundation;
}

/*
 * PropSet are just any Game Objects that can be used as props around the map,
 * all of them will be used randomly/uniformly, no pretermined types
 */
[System.Serializable]
public struct PropSet
{
    public GameObject[] propPrefabs;
}


[CreateAssetMenu(fileName = "SO_MapGenData", menuName = "SOs/MapGen/SO_MapGenData")]
public class SO_MapGenData : ScriptableObject
{
    [Space(30)]
    [Header("    _____________  MAP SET-UP  _____________")]
    [Space(5)]
    public VolumeProfile ppProfile;

    [Header("BLOCK SET ")]
    [Space(10)]
    public BlockSet blockSet;

    [Space(10)]
    public PropSet propSet;

    [Space(30)]
    [Header("    _____________  MAP BASE  _____________")]
    [Space(40)]
    [MinMaxSlider(-1f, 1f)]
    public Vector2 randomElevation;

    [Header("PATH SIZE ")]
    public float distanceOfPath = 40f;

    [Header("PATH TWISTING ")]
    [MinMaxSlider(0f, 30f)]
    public Vector2 distanceBeforeTurningPath;

    [MinMaxSlider(0f, 100f)]
    public Vector2 turningAngleRange;

    [Header("PATH THICKNESS ")]
    public int pathThickenFrequency = 8;

    [MinMaxSlider(0f, 50f)]
    public Vector2 grassFillRadius;

    [Header("The higher, the more square-ish")]
    [Range(1.0f, 1.5f)]
    public float circularity = 1.0f;

    [Header("LUMPS ")]
    [Space(10)]
    [Range(0, 40)]
    public int lumpDensity;

    public int lumpApplicationRounds = 3;

    [MinMaxSlider(0f, 100f)]
    public Vector2 lumpRadius;

    public int lumpOffset = 2;

    public bool addEdgingBlocks = false;

    [Header("Elevation smoothening (flatten)")]
    public int flattenApplicationRounds = 1;
    [Range(0, 9)]
    public int flattenIfSurroundedByLessThan = 3;

    [Header("DIRT PATH ")]
    [Space(10)]
    public bool raiseDirtLevel = true;

    public int dirtPathThickenFrequency = 8;

    [MinMaxSlider(0f, 40f)]
    public Vector2 dirtFillRadius;

    public int dirtCutoffFrequency;

    [MinMaxSlider(0f, 12f)]
    public Vector2 dirtCutoffLength;

    [Space(30)]
    [Header("    _____________  MAP PROPS  _____________")]
    [Space(40)]
    public bool showProps;

    [Header("TREE SPAWNING ")]
    [Range(0, 10)]
    public int treeSpawnDensity = 4;
    public int treeSpawnOffset = 2;
    public float treeChanceGrowthRate = 5.0f;

    [Space(30)]
    [Header("    _____________  ENEMIES  _____________")]
    [Space(40)]
    public GameObject[] enemies;
    public int enemyFrequency;
    public int enemySpawnOffset;
    public int spawnDoubleEveryFrequency;
}

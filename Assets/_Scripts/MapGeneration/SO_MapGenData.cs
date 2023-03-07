using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "SO_MapGenData", menuName = "SOs/MapGen/SO_MapGenData")]
public class SO_MapGenData : ScriptableObject
{
    [Space(30)]
    [Header("    _____________  MAP LORE  _____________")]
    [Space(5)]
    public string mapDescription;

    [Space(30)]
    [Header("    _____________  MAP MECHANICS  _____________")]
    [Space(5)]
    public GameObject mapMechanics;
    public ParticleSystem mapMechanicsVFX;

    [Space(30)]
    [Header("    _____________  MAP SET-UP  _____________")]
    [Space(5)]
    public VolumeProfile ppProfile;
    public ParticleSystem vfx;

    [Header("BLOCK SET ")]
    [Space(10)]
    public BlockSet blockSet;

    [Space(10)]
    public PropSet propSet;

    [Space(30)]
    [Header("    _____________  MAP BASE  _____________")]
    [Space(5)]
    [MinMaxSlider(-1f, 1f)]
    public Vector2 randomElevation;

    [Header("PATH SIZE "), MinMaxSlider(0f, 300)]
    public Vector2 distanceOfPath;

    [Header("PATH TWISTING ")]
    [MinMaxSlider(0f, 30f)]
    public Vector2 distanceBeforeTurningPath;

    [MinMaxSlider(0f, 100f)]
    public Vector2 turningAngleRange;

    [Range(0f, 90f)]
    public float roundAngleToNearest;

    [Header("Path Overlap Prevention Tech")]
    [Header("(recommended max: 90f)")]
    [Range(0f, 90f)]
    public float maxAccumulatedAngle;

    [Header("PATH THICKNESS ")]
    public int pathThickenFrequency = 8;

    [MinMaxSlider(0f, 50f)]
    public Vector2 grassFillRadius;

    [Header("The higher, the more square-ish")]
    [Range(1.0f, 1.5f)]
    public float circularity = 1.0f;

    [Space(30)]
    [Header("    _____________  MAP LUMPS  _____________")]
    [Space(5)]

    [Range(0, 1)]
    [Tooltip("Block elevation distance, a whole block up or half? Etc.")]
    public float blockRaiseSize = 1f;

    [Range(0, 1)]
    [Tooltip("Above, except this is for edge blocks")]
    public float edgeBlockRaiseSize = 1f;

    [Range(0, 40)]
    public int lumpDensity;

    public int lumpApplicationRounds = 3;

    [MinMaxSlider(0f, 100f)]
    public Vector2 lumpRadius;

    public int lumpOffset = 2;

    [Header("Edges & Foundations")]
    public bool addEdgeWalls;
    public bool addEdgeFoundation;
    public int sizeOfFoundation;
    public int sizeOfWall;

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
    [Header("    _____________  MAP LIQUID  _____________")]
    [Space(5)]
    public float liquidRiseLevel;

    [Space(30)]
    [Header("    _____________  MAP PROPS  _____________")]
    [Space(5)]
    public bool showProps;

    [Range(0, 40)]
    public int foliageDensity;

    [Header("TREE SPAWNING ")]
    [Range(0, 10)]
    public int treeSpawnDensity = 4;
    public int treeSpawnOffset = 2;
    public float treeChanceGrowthRate = 5.0f;

    [Space(30)]
    [Header("    _____________  ENEMIES  _____________")]
    [Space(5)]
    public bool addEnemies;
    public GameObject[] enemies;
    public int enemySpawnAmount;
    public int enemySpawnOffset;

    [Space(30)]
    [Header("    ___________  INTERACTABLES  ____________")]
    [Space(5)]
    public bool addChests;
    public bool addBlacksmith;
    public bool addMerchant;


}

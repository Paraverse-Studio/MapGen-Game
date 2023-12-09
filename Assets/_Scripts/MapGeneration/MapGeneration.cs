/*
 * Copyright (c) Abhishek Mohan
 * https://github.com/AbhishekMohan/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.AI;
using Paraverse.Mob.Stats;
using Paraverse.Mob.Controller;
using Paraverse.Player;

[System.Serializable]
public class PropItem
{

}

[System.Serializable]
public struct BlockSet
{
    public SO_BlockItem grass;
    public SO_BlockItem dirt;
    public SO_BlockItem water;
    public SO_BlockItem foundation;
    public SO_BlockItem bridge;
}

[System.Serializable]
public struct PropSet
{
    public GameObject[] propPrefabs;
}

public class MapGeneration : MonoBehaviour
{
    public enum Side
    {
        north, south
    }

    public enum ElevationLevel
    {
        highest, lowest
    }

    public enum ElevateObject
    {
        up, down
    }


    [System.Serializable]
    public struct ImportantProps
    {
        public GameObject endPoint;
    }

    [System.Serializable]
    public struct GridOccupant
    {
        public Block block;
        public bool hasProp;
        public bool hasWater;
    }

    public static MapGeneration Instance;

    [Header("Map Generation Data ")]
    public SO_MapGenData M;
    [Header("Post Processing Data ")]
    public Volume globalVolume;
    public Light globalLight;

    [Header("NavMesh Data ")]
    public NavMeshBuilder navMeshBuilder;

    [Header("Water Splash VFX"), SerializeField]
    private ParticleSystem waterSplash;

    [Header("Randomness seed ")]
    public int randomSeed;
    [Space(10)]

    public GameObject blockPrefab;

    [Header("Important Props")]
    public ImportantProps importantProps;

    public Transform blocksFolder;
    public Transform enemiesFolder;

    private GameObject temporaryObjFolder;
    public GameObject[] foundationPrefabs;

    [Header("(Experimental) Preloader")]
    public GameObject[] preloadingObjects;

    [Header("Line (GPS)")]
    public LineRenderer line;
    public bool drawLine = true;
    public bool lineSmoothening = true;
    [Space(10)]

    [Header("Processing Delay")]
    public float processesDelay = 0.02f;

    [Range(0f, 1f)]
    public float delayAfterPercentDeleted = 0.06f;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI qualityText;

    [Space(25)]
    public UnityEvent OnMapGenerateStart = new UnityEvent();
    public UnityEvent OnMapGenerateEnd = new UnityEvent();
    public UnityEvent OnScreenReady = new UnityEvent();

    public FloatFloatEvent OnProgressChange = new FloatFloatEvent();
    public StringEvent OnProgressChangeText = new StringEvent();


    #region SETTINGS_VARIABLES
    private float _EPSILON = 0.002f;

    private int _GRIDSIZE; // should be double of distance
    private Vector3 centerPoint;
    public Vector3 CenterPoint => centerPoint;
    private Vector2 centerPoint2D;
    private Vector3 centerPointWithY;
    public Vector3 CenterPointWithY => centerPointWithY;
    private SO_BlockItem currentPaintingBlock;
    #endregion


    #region RUNTIME_VARIABLES
    private float runtimeDistanceOfPath;
    private float pathingAngle;
    private float distanceCreated = 0;

    private GridOccupant[,] gridOccupants;

    private List<Block> allObjects = new List<Block>(); // all ground-related blocks
    private List<Block> pathObjects = new List<Block>();
    private List<GameObject> propObjects = new List<GameObject>();
    private List<GameObject> foundationObjects = new List<GameObject>();
    private List<Block> waterObjects = new List<Block>();
    private List<GameObject> enemyObjects = new List<GameObject>();

    private ParticleSystem mapVFX = null;
    private IMapMechanics mapMechanics = null;

    private float progressValue;
    private float progressTotal = 11f;
    private int step = 0; // purely for debugging to detect step progress speed
    private System.DateTime testingTime;

    private WaitForSeconds processDelay;

    // Need to be initialized
    private Vector2 xBoundary;
    private Vector2 zBoundary;
    private Vector2 yBoundary;
    public Vector2 YBoundary => yBoundary;

    private Vector2 furthestBlock;
    private float furthestDistance = 0f;
    #endregion

    private void Awake()
    {
        Instance = this;
        processDelay = new WaitForSeconds(processesDelay);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();

        if (Input.GetKeyDown(KeyCode.F10)) 
            GlobalSettings.Instance.waterVolume.gameObject.SetActive(!GlobalSettings.Instance.waterVolume.gameObject.activeSelf);
    }

    public void RegenerateMap() => StartCoroutine(ERenegerateMap());

    public IEnumerator ERenegerateMap()
    {
        testingTime = System.DateTime.Now;

        OnMapGenerateStart?.Invoke();
        //PartitionProgress("Clearing & recycling resources...");

        yield return processDelay;

        ClearMap();

        OnMapGenerateStart?.Invoke();

        yield return processDelay;

        StartCoroutine(Generation());
    }

    public void ClearMap()
    {
        ResetVariables();
        ResetLists();
    }

    private void ResetVariables()
    {
        // Used to be in Start()
        runtimeDistanceOfPath = Random.Range(M.distanceOfPath.x, M.distanceOfPath.y);
        _GRIDSIZE = (int)((runtimeDistanceOfPath * 2.0f) + (M.grassFillRadius.y * 2.0f) + 1);

        // Can't and shouldn't change these 2 variables mid-play, only one time per play session
        gridOccupants = new GridOccupant[_GRIDSIZE, _GRIDSIZE];
        centerPoint = new Vector3((int)(_GRIDSIZE / 2), 0, (int)(_GRIDSIZE / 2));
        centerPoint2D = new Vector3(centerPoint.x, centerPoint.z);

        int randomSeedNumber = randomSeed > -1 ? randomSeed : Random.Range(0, 99999);
        Random.InitState(randomSeedNumber);
        seedText.text = "Seed  " + randomSeedNumber;
        qualityText.text = "Quality Level: " + QualityManager.QualityLevel;

        progressValue = -1f;

        xBoundary = centerPoint2D;
        zBoundary = centerPoint2D;

        yBoundary = new Vector2(Mathf.Ceil(M.lumpApplicationRounds / 2.0f) * M.blockRaiseSize,
                                Mathf.Floor(-M.lumpApplicationRounds / 2.0f) * M.blockRaiseSize);

        furthestBlock = centerPoint2D;
        furthestDistance = 0;

        distanceCreated = 0;
        pathingAngle = 0f; // Random.Range(0f, 360f);

        line.positionCount = 0;
        currentPaintingBlock = M.blockSet.grass;
    }

    private IEnumerator DestroyChildren(Transform t)
    {
        int totalObjectsToDelete = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[t.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in t)
        {
            allChildren[totalObjectsToDelete] = child.gameObject;
            totalObjectsToDelete += 1;
        }

        float percentOfObjectsAfterWhichUseADelay = delayAfterPercentDeleted;
        int limit = (int)(totalObjectsToDelete * percentOfObjectsAfterWhichUseADelay);
        int counter = 0;

        //Now destroy them
        for (int i = 0; i < allChildren.Length; ++i)
        {
            DestroyImmediate(allChildren[i]);
            counter++;

            if (counter >= limit)
            {
                counter = 0;
                yield return processDelay;
            }           
        }

        if (t.gameObject) DestroyImmediate(t.gameObject);
    }

    private void ResetLists()
    {
        /*
        if (false)
        {
            #region OBJECT POOL DECOMMISSIONING
            // Resetting object lists
            for (int x = 0; x < _GRIDSIZE; ++x)
            {
                for (int z = 0; z < _GRIDSIZE; ++z)
                {
                    if (gridOccupants[x, z].block != null)
                    {
                        gridOccupants[x, z].block.gameObject.SetActive(false);
                        gridOccupants[x, z].block = null;
                    }
                }
            }

            allObjects.Clear();
            pathObjects.Clear();

            if (treeObjects.Count > 0)
            {
                for (int i = treeObjects.Count - 1; i >= 0; --i)
                {
                    Destroy(treeObjects[i]);
                }
            }
            treeObjects.Clear();

            //PartitionProgress();
            //yield return null;

            if (foundationObjects.Count > 0)
            {
                for (int i = foundationObjects.Count - 1; i >= 0; --i)
                {
                    foundationObjects[i].SetActive(false);
                }
            }
            foundationObjects.Clear();

            if (waterObjects.Count > 0)
            {
                for (int i = waterObjects.Count - 1; i >= 0; --i)
                {
                    waterObjects[i].CurrentPrefab.transform.parent = Pool.Instance.gameObject.transform;
                    waterObjects[i].gameObject.SetActive(false);
                }
            }
            waterObjects.Clear();

            //enemyObjects.Clear();

            // Important Props

            #endregion
        }
        */
        if (true)
        {
            #region DESTROY DECOMMISSIONING

            GameObject referenceFolder = temporaryObjFolder;
            if (referenceFolder) StartCoroutine(DestroyChildren(referenceFolder.transform));

            allObjects.Clear();
            pathObjects.Clear();
            propObjects.Clear();
            foundationObjects.Clear();
            waterObjects.Clear();
            for (int i = 0; i < enemyObjects.Count; ++i) if (enemyObjects[i]) Destroy(enemyObjects[i]);
            enemyObjects.Clear();

            if (mapVFX) Destroy(mapVFX.gameObject);
            // Important Props

            temporaryObjFolder = new GameObject("Folder");
            temporaryObjFolder.transform.parent = blocksFolder;

            for (int i = 0; i < preloadingObjects.Length; ++i)
            {
                if (null == preloadingObjects[i]) continue;
                GameObject g = Instantiate(preloadingObjects[i]);
                Destroy(g, Random.Range(0.1f, 0.5f));
            }

            mapMechanics?.Clear();

            #endregion
        }            
    }

    private IEnumerator Generation()
    {
        PartitionProgress("Initiating generation process...");
        yield return processDelay;

        /* * * * * CREATION OF MAP * * * * */

        currentPaintingBlock = M.blockSet.grass;

        SpawnPath();
        step = 1;
        PartitionProgress("Shaping map...");
        yield return processDelay;

        ThickenPath();
        step = 2;
        PartitionProgress();
        yield return processDelay;

        ThickenAroundObject(pathObjects[pathObjects.Count - 1].gameObject, 0, M.grassFillRadius);
        step = 3;
        PartitionProgress();
        yield return processDelay;

        /* * * * * SHAPE MODIFICATION OF MAP * * * * */

        AddRandomLumps();
        step = 4;
        PartitionProgress();
        yield return processDelay;

        // Isn't really working - if you flatten, then the boxes have new weird lumps
        // to flatten again... and flattening again would cause it again
        //SmoothenElevation(M.flattenApplicationRounds);
        //PartitionProgress();
        //yield return processDelay;



        /* * * * * * PAINTING ON SHAPE OF MAP * * * * * */

        currentPaintingBlock = M.blockSet.dirt;

        PartitionProgress("Adding world props...");
        yield return processDelay;
        PaintDirtPath();
        step = 5;

        if (M.randomElevation.x != M.randomElevation.y) ApplyRandomElevation();
        //PartitionProgress("Activating props...");
        //yield return processDelay;

        if ((M.addEdgeFoundation || M.addEdgeWalls) && (QualityManager.QualityLevel >= 3))
        {
            AddFoundationAndEdgeWork();
            PartitionProgress();
            step = 55;
            yield return processDelay;
        }

        if (M.raiseDirtLevel) 
        {
            RaiseBlocksAboveLiquid();
        }

        /////////       NO SHAPE MODIFICATIONS BEYOND THIS POINT        /////////////////////////
        centerPointWithY = new Vector3(centerPoint.x, gridOccupants[(int)centerPoint.x, (int)centerPoint.z].block.transform.position.y, centerPoint.z);
                 

        /* * * * * DECORATIVE/MECHANICAL PROPS ON MAP * * * * * * */
        currentPaintingBlock = M.blockSet.water;

        PartitionProgress("");
        step = 7;
        yield return processDelay;
        AddWaterToDips();

        /* * * * * IMPORTANT PROPS ON MAP * * * * * * */
        PartitionProgress("Adding event triggers...");
        step = 6;
        yield return processDelay;
        AddImportantProps();

        /* * * * * DECORATIVE PROPS ON MAP * * * * * * */
        step = 8;
        PartitionProgress("");
        yield return processDelay;
        if (QualityManager.QualityLevel > 3) AddFoliage();        

        step = 9;
        PartitionProgress("");
        yield return processDelay;
        AddProps();


        /* * * * * * ADDING ENEMIES * * * * * * * * * */

        if (M.addEnemies)
        {
            step = 10;
            PartitionProgress("Spawning dangerous enemies...");
            yield return processDelay;

            // Based on testing, NavMeshSurface isn't needed on every block, just 1, and then it bakes navmesh map
            navMeshBuilder.surface = allObjects[0].CheckNavMeshSurface(allObjects[0].CurrentPrefab);
            navMeshBuilder.BuildNavMesh();

            AddEnemies();            
            enemiesFolder.gameObject.name = "[ Enemies ] - " + enemyObjects.Count;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * */


        /* * * * * MAP MECHANICS - LAVA INTERACTION, WATER INTERACTION, ETC. * * * * * */

        mapMechanics = Instantiate(M.mapMechanics, transform).GetComponent<IMapMechanics>();
        mapMechanics.Initialize(M.mapMechanicsVFX);


        /* * * * MISC STEPS - FINAL TOUCHES * * * * * * */
        step = 11;
        PartitionProgress("Applying final touches...");
        yield return processDelay;

        if (M.ppProfile) globalVolume.profile = M.ppProfile;

        TeleportPlayer(CenterPointWithY + new Vector3(0, 5f, 0));

        if (M.vfx)
        {
            mapVFX = Instantiate(M.vfx, Vector3.zero, Quaternion.identity);
        }
        /* * * * * * * * * * * * * * * * * * * * * * */


        /* * * * * QUALITY SETTINGS (PERFORMANCE) * * * * * * */
        //globalLight.gameObject.SetActive(QualityManager.QualityLevel >= 2);
        //globalVolume.gameObject.SetActive(QualityManager.QualityLevel >= 3);
        QualitySettings.SetQualityLevel(Mathf.Max(0, QualityManager.QualityLevel - 1), true);
        if (QualityManager.QualityLevel <= 4 && mapVFX) Destroy(mapVFX.gameObject);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * */

        {  // debug
            var timeTakenToGenerateMap = System.DateTime.Now - testingTime;
            Debug.Log($"Map Generation: It took {((int)(timeTakenToGenerateMap.TotalSeconds * 100f)) / 100f} seconds to generate this map!");
        }

        OnMapGenerateEnd?.Invoke();
        yield return new WaitForSeconds(0.8f);
        OnScreenReady?.Invoke();
    }

    private void SpawnPath()
    {
        while (distanceCreated < runtimeDistanceOfPath)
        {
            float randomAngle = Random.Range(M.turningAngleRange.x, M.turningAngleRange.y);

            // this was a feature I was trying - rounds the angle to the nearest 45deg
            // ie. only angle turns are 0, 45, 90, etc.
            randomAngle = Mathf.Round(randomAngle / M.roundAngleToNearest) * M.roundAngleToNearest;
            randomAngle *= (Random.value > 0.5f) ? 1.0f : -1.0f;

            // New Feature* : trying out preventing the pathing curve to ever rotate enough to face the origin spot
            // so that we don't get overlap path ways
            if (pathingAngle >= M.maxAccumulatedAngle) randomAngle = Mathf.Abs(randomAngle) * -1f;
            else if (pathingAngle <= -M.maxAccumulatedAngle) randomAngle = Mathf.Abs(randomAngle);

            float newAngle = pathingAngle + randomAngle;

            float randomDistance = Random.Range(M.distanceBeforeTurningPath.x, M.distanceBeforeTurningPath.y);

            // Building the very first object
            if (allObjects.Count == 0)
            {
                Block obj = Spawn(centerPoint);
                if (obj)
                {
                    allObjects.Add(obj);
                    pathObjects.Add(obj);                    
                }
            }
            else // Buiolding the rest of the path
            {
                SpawnThroughPath(allObjects[allObjects.Count - 1].gameObject, newAngle, randomDistance);
            }
        }
    }

    private void SpawnThroughPath(GameObject turningPointObj, float newAngle, float scale)
    {
        int blocksCreated = 0;
        for (float i = 0.6f; i <= scale; i += 0.6f)
        {
            Block newObj = Spawn(GetPointOnCircle(turningPointObj.transform.position, i, newAngle));

            if (newObj)
            {
                allObjects.Add(newObj);
                pathObjects.Add(newObj);

                pathingAngle = newAngle;
                blocksCreated += 1;
            }
        }

        distanceCreated += Vector3.Distance(pathObjects[pathObjects.Count - 1]
                    .transform.position, pathObjects[pathObjects.Count - 1 - blocksCreated].transform.position);
    }

    // From a center object, this loops through all 360 angles, 20 at a
    // time, and draws objects to fill the circle
    private void ThickenPath()
    {
        for (int i = 0; i < pathObjects.Count; i++)
        {
            ThickenAroundObject(pathObjects[i].gameObject, i, M.grassFillRadius);
        }
    }

    // Given a cube obj, add cubes around it to form a circle of cubes of radius fillRadius
    private void ThickenAroundObject(GameObject obj, int i, Vector2 fillRadius)
    {
        // Determining what type of circle fill
        float thickness;
        if (i % M.pathThickenFrequency != 0)
        {
            thickness = fillRadius.x;
        }
        else
        {
            thickness = Random.Range(fillRadius.x, fillRadius.y);
        }

        // Randomizing the circle radius' off-set a bit
        int randomizingCap = (int)(System.Math.Min(fillRadius.x, 3));
        int randomizedX = Random.Range(-randomizingCap, randomizingCap);
        int randomizedZ = Random.Range(-randomizingCap, randomizingCap);
        Vector3 centerObjectOffsetted = obj.transform.position + ((currentPaintingBlock == M.blockSet.dirt)? Vector3.zero : new Vector3(randomizedX, 0, randomizedZ));

        // Looping through all areas in the circle, and spawning another block
        for (float x = -thickness; x < thickness; x += 1f)
        {
            for (float z = -thickness; z < thickness; z += 1f)
            {
                Vector3 newSpot = centerObjectOffsetted + new Vector3((int)x, 0, (int)z);

                // This condition is for circularity, helps form a circle around the obj, instead a rigid square
                if (IsDistanceLessThan(centerObjectOffsetted, newSpot, (thickness * M.circularity))) // tags: circular
                {
                    Block replacedBlock = null;
                    Block block = SpawnAdvanced(newSpot, ref replacedBlock);

                    if (block) allObjects.Add(block);

                    // This part here is because we have a special condition to apply to dirt paths
                    // which is to never have them on the lowest elevation layer, and if they are, elevate them
                    // (design choice: don't want dirt to be covered with water - dirt paths should always be clear to walk on)
                    if (M.raiseDirtLevel && null != replacedBlock && replacedBlock.type == M.blockSet.dirt)
                    {                        
                        //ApplyBlockElevationRestrictions(replacedBlock);
                    }
                }
            }
        }
    }

    private void RaiseBlocksAboveLiquid()
    {
        for (float i = 0; i <= (M.liquidRiseLevel - YBoundary.y)/M.blockRaiseSize; ++i)
        {
            foreach (Block b in allObjects)
            {
                if (b.type != M.blockSet.dirt) continue;

                if (b.transform.position.y <= M.liquidRiseLevel)
                {
                    ElevateBlock(upOrDown: true, b);
                }
            }
        }
    }

    private void SmoothRaise(bool upOrDown, Vector3 area, float radius)
    {
        if (radius <= 0f) return;

        for (float x = -radius; x < radius; x += 1f)
        {
            for (float z = -radius; z < radius; z += 1f)
            {
                Vector3 newSpot = area + new Vector3(x, 0, z);

                if (!IsInGrid(new Vector3((int)newSpot.x, 0, (int)newSpot.z))) continue;

                Block potentialObj = gridOccupants[(int)newSpot.x, (int)newSpot.z].block;

                if (potentialObj != null)
                {
                    float factor = 0.11f;

                    ElevateBlock(upOrDown, potentialObj, factor, ignoreBounds: true);
                    //potentialObj.transform.position += new Vector3(0, (upOrDown? 1f : -1f)* factor, 0);
                    //ElevateBlock(upOrDown, potentialObj);
                }
            }
        }
        SmoothRaise(upOrDown, area, radius - 1f);
    }

    private void AddRandomLumps()
    {
        //// By default, lumps will spawn in a grid fashion
        // Spread determines how far lumps are apart (spread = 2, means a lump will appear every 2 blocks)
        int spread2 = M.lumpDensity;

        for (int upOrDown = 0; upOrDown < M.lumpApplicationRounds; ++upOrDown)
        {
            for (int x = (int)xBoundary.x; x < xBoundary.y; x += spread2)
            {
                for (int z = (int)zBoundary.x; z < zBoundary.y; z += spread2)
                {
                    if (!IsInGrid(new Vector3((int)x, 0, (int)z)) || null == gridOccupants[x, z].block) continue;

                    int randomXOffset = Random.Range(-M.lumpOffset, M.lumpOffset + 1);
                    int randomZOffset = Random.Range(-M.lumpOffset, M.lumpOffset + 1);

                    x += randomXOffset;
                    z += randomZOffset;

                    //SmoothRaise(z % 2 == 0 ? true : false, new Vector3(x, 0, z), Random.Range(M.lumpRadius.x, M.lumpRadius.y));
                    ElevateCircle(z % 2 == 0 ? true : false, new Vector3(x, 0, z), Random.Range(M.lumpRadius.x, M.lumpRadius.y));
                }
            }
        }


        if (false)
        {
            //// By default, lumps will spawn in a grid fashion
            // Spread determines how far lumps are apart (spread = 2, means a lump will appear every 2 blocks)
            int spread = M.lumpDensity;

            for (int upOrDown = 0; upOrDown < M.lumpApplicationRounds; ++upOrDown)
            {
                for (int x = (int)xBoundary.x; x < xBoundary.y; x += spread)
                {
                    for (int z = (int)zBoundary.x; z < zBoundary.y; z += spread)
                    {
                        if (!IsInGrid(new Vector3((int)x, 0, (int)z)) || null == gridOccupants[x, z].block) continue;

                        int randomXOffset = Random.Range(-M.lumpOffset, M.lumpOffset + 1);
                        int randomZOffset = Random.Range(-M.lumpOffset, M.lumpOffset + 1);

                        x += randomXOffset;
                        z += randomZOffset;

                        ElevateCircle(z % 2 == 0 ? true : false, new Vector3(x, 0, z), Random.Range(M.lumpRadius.x, M.lumpRadius.y));
                    }
                }
            }
        }
    }

    private void ElevateCircle(bool upOrDown, Vector3 area, float radius)
    {
        List<Block> alreadyElevated = new List<Block>();

        // Looping through all areas in the circle, and spawning another block
        for (float x = -radius; x < radius; x += 1f)
        {
            for (float z = -radius; z < radius; z += 1f)
            {
                Vector3 newSpot = area + new Vector3(x, 0, z);

                //if (Vector3.Distance(area, newSpot) < (radius* M.circularity)) // tags: circular
                if (IsDistanceLessThan(area, newSpot, (radius * M.circularity)))
                {
                    if ((int)newSpot.x < 0 || (int)newSpot.x >= _GRIDSIZE) continue; // can happen if the radius for this lump is bigger than the actual map
                    if ((int)newSpot.z < 0 || (int)newSpot.z >= _GRIDSIZE) continue;

                    Block potentialObj = gridOccupants[(int)newSpot.x, (int)newSpot.z].block;

                    if (potentialObj != null && !alreadyElevated.Contains(potentialObj))
                    {
                        ElevateBlock(upOrDown, potentialObj);
                    }
                }
            }
        }
        alreadyElevated.Clear();
    }

    // Given an object obj, and an elevation direction, lift/lower the object in that direction BUT
    // with the condition that no block around this block in 3x3 should end up 2-block vertically distanced
    private void ElevateBlock(bool upOrDown, Block obj, float overrideElevationSize = -1f, bool ignoreBounds = false)
    {
        float extremeY;
        extremeY = GetExtremestAdjacentElevation(upOrDown ? ElevationLevel.lowest : ElevationLevel.highest, obj.gameObject);

        float elevationSize = M.blockRaiseSize;
        if (overrideElevationSize != -1) elevationSize = overrideElevationSize;

        float yValue = extremeY + ((upOrDown ? 1f : -1f) * elevationSize);

        // this is so that these elevations don't create new extreme ends. That is to be determined purely from lump application rounds
        if (!ignoreBounds) yValue = Mathf.Clamp(yValue, YBoundary.y, YBoundary.x); 

        obj.transform.position = new Vector3(obj.transform.position.x, yValue, obj.transform.position.z);

        obj.UpdateHistory((upOrDown ? "Raised" : "Lowered") + " to " + obj.transform.position);
    }

    private Block Spawn(Vector3 vec, bool utilizeY = false)
    {
        Block replacementBlock = null;
        return SpawnAdvanced(vec, ref replacementBlock, utilizeY);
    }

    private Block SpawnAdvanced(Vector3 vec, ref Block replacedBlock, bool utilizeY = false)
    {
        // majority of the time this will be left as false since you're only affecting grid[x,z]
        // this should be true, when you're spawning a cube at x,z and supp
        if (!utilizeY) // majority of the time you use Spawn(), you ignore y, cause you're affecting grid[x,z]
        {
            Block blockAtVec = null;

            if (IsInGrid(new Vector3((int)vec.x, -1f, (int)vec.z))) // the y doesn't matter here, IsInGrid() doesn't use it
            {
                blockAtVec = gridOccupants[(int)vec.x, (int)vec.z].block;
            }

            if (null != blockAtVec)
            {
                replacedBlock = blockAtVec;
                blockAtVec.type = currentPaintingBlock;
                blockAtVec.UpdateBlock();
                return null;
            }
        }

        Vector3 spawnSpot = new Vector3((int)vec.x, (int)vec.y, (int)vec.z);

        GameObject obj = Instantiate(blockPrefab, spawnSpot, Quaternion.identity);

        obj.transform.parent = temporaryObjFolder.transform;
        Block block = obj.GetComponent<Block>();
        block.type = currentPaintingBlock;
        block.UpdateBlock();
        block.UpdateHistory("Spawned at " + spawnSpot);

        if (IsInGrid(new Vector3((int)vec.x, 0, (int)vec.z))) gridOccupants[(int)vec.x, (int)vec.z].block = block;

        if (utilizeY) return block;

        UpdateBoundaryStats(obj.transform.position);
        return block;
    }

    private bool IsOnSideOfBlock(Side side, GameObject src, GameObject subject)
    {
        Vector3 angle45 = new Vector3(-1, 0, 1);
        Vector3 angleToSubject = subject.transform.position - src.transform.position;

        float thisAngle = Vector3.Angle(angle45, angleToSubject);
        return (side == Side.north ? thisAngle <= 70f : thisAngle > 110f);
    }

    // Given a block (src) and a elevation type (up or down),
    // return the extremest Y value found in the 3x3 block radius around src
    // (ex. lowest level, return the lowest y out of the 9 blocks)
    private float GetExtremestAdjacentElevation(ElevationLevel level, GameObject src)
    {
        float extremestY = src.transform.position.y;

        for (int x = -1; x < 2; ++x)
        {
            for (int z = -1; z < 2; ++z)
            {
                Vector3 areaToCheck = new Vector3(src.transform.position.x + x, 0, src.transform.position.z + z);

                if (!IsInGrid(new Vector3((int)areaToCheck.x, 0, (int)areaToCheck.z)) 
                    || null == gridOccupants[(int)areaToCheck.x, (int)areaToCheck.z].block) continue;

                Block objectToCheck = gridOccupants[(int)areaToCheck.x, (int)areaToCheck.z].block;
                if (objectToCheck != null)
                {
                    if (level == ElevationLevel.highest && objectToCheck.transform.position.y > extremestY)
                    {
                        extremestY = objectToCheck.transform.position.y;
                    }
                    else if (level == ElevationLevel.lowest && objectToCheck.transform.position.y < extremestY)
                    {
                        extremestY = objectToCheck.transform.position.y;
                    }
                }
            }
        }
        return extremestY;
    }

    // Given a block (vector3 src), return the number of blocks that exist in its 3x3 radius 
    // Specify sameElevation if only counting those blocks that are on the same y-level
    // includes itself's position (result will always be 1 at lowest)
    private int NumOfAdjacentOccupied(Vector3 src, bool sameElevationNeeded = false)
    {
        int sidesCovered = 0;
        for (int x = -1; x < 2; ++x)
        {
            for (int z = -1; z < 2; ++z)
            {
                Vector3 areaToCheck = new Vector3(src.x + x, 0, src.z + z);

                if (!IsInGrid(areaToCheck)) continue;

                Block objectToCheck = gridOccupants[(int)areaToCheck.x, (int)areaToCheck.z].block;

                if (objectToCheck != null)
                {
                    if (sameElevationNeeded)
                    {
                        if (Mathf.Abs(src.y - objectToCheck.transform.position.y) < _EPSILON)
                        {
                            sidesCovered++;
                        }
                    }
                    else sidesCovered++;
                }
            }
        }
        return sidesCovered;
    }

    // Once the base map has been generated, iterate from pathObjects (base line),
    // and paint over blocks to be dirt blocks
    private void PaintDirtPath()
    {
        for (int i = 0; i < pathObjects.Count; i++)
        {
            // Cuts dirt path - if a cut is to happen (frequency met), then skip i to dirtCutLength
            if (i % M.dirtCutoffFrequency == 0)
            {
                int dirtCutLength = (int)Random.Range(M.dirtCutoffLength.x, M.dirtCutoffLength.y);
                i += dirtCutLength;
                if (i >= pathObjects.Count) return;
            }

            pathObjects[i].type = M.blockSet.dirt;
            ThickenAroundObject(pathObjects[i].gameObject, i, M.dirtFillRadius);
        }
    }

    // Goes through all ground blocks and raises/lowers their y-value by a small amount
    // to give that blocky/cubica look
    // THIS NEEDS TO HAPPEN LAST (in steps of Map Generation)
    private void ApplyRandomElevation()
    {
        for (int i = 0; i < allObjects.Count; i++)
        {
            //if (allObjects[i].type != blocks.dirt) continue;

            float elevation = Random.Range(M.randomElevation.x, M.randomElevation.y);
            float xRandom = 0; // Random.Range(-M.randomElevation.x / 2f, M.randomElevation.x / 2f);
            float zRandom = 0; // Random.Range(-M.randomElevation.x / 2f, M.randomElevation.x / 2f);

            allObjects[i].transform.position += new Vector3(xRandom, elevation, zRandom);
        }
    }

    private void AddWaterToDips()
    {
        float yLevelToMeasure = M.liquidRiseLevel + _EPSILON;

        for (int i = 0; i < allObjects.Count; ++i)
        {
            Transform thisObject = allObjects[i].transform;

            if (thisObject.position.y <= yLevelToMeasure)
            {
                Vector3 spawnSpot = new Vector3(thisObject.position.x, YBoundary.y + M.blockRaiseSize, thisObject.position.z);

                GameObject waterObj = Instantiate(blockPrefab, spawnSpot, Quaternion.identity);
                waterObj.transform.parent = temporaryObjFolder.transform;
                if (waterObj)
                {
                    Block waterBlock = waterObj.GetComponent<Block>();
                    waterBlock.type = M.blockSet.water;
                    waterBlock.UpdateBlock();
                    waterObjects.Add(waterBlock);
                    allObjects[i].hasWater = true;
                }
            }
        }

        // modify this next bit to make water more adaptive and dynamic to whatever size and height you need it
        WaterVolumeFollowTarget.Instance.overrideY = M.liquidRiseLevel + (M.blockRaiseSize * 0.5f);
    }

    private void AddImportantProps()
    {
        // EndPoint
        Vector3 spawnSpot = pathObjects[pathObjects.Count - 1].transform.position + new Vector3(0, 0.5f, 0);
        GameObject obj = Instantiate(importantProps.endPoint, spawnSpot, GetCameraFacingRotation(reverse: true));
        obj.name = "END PORTAL (Special)";
        UtilityFunctions.UpdateLODlevels(obj.transform);
        GameLoopManager.Instance.EndPortal = obj.GetComponent<EndPointTrigger>();
        propObjects.Add(obj);
        obj.transform.parent = temporaryObjFolder.transform;

        // Chests
        if (M.addChests)
        {
            int numChests = Random.Range((int)M.numOfChests.x, (int)M.numOfChests.y + 1);
            int blocksGapBetweenChests = pathObjects.Count / (numChests + 1);

            int runtimeDistance = 0;
            int chestsSpawned = 0;
            for (int i = 0; i < pathObjects.Count; ++i)
            {
                runtimeDistance++;

                if (runtimeDistance >= blocksGapBetweenChests)
                {          
                    Block b = pathObjects[i];
                    Vector3 randomOffset = new(Random.Range(2, 9f), 0, Random.Range(2, 9f));
                    b = GetClosestValidGroundBlock(b.transform.position + randomOffset);

                    var chest = Instantiate(MapCreator.Instance.chestPrefab, b.transform.position + new Vector3(0, 0.5f, 0), GetCameraFacingRotation());
                    chest.Initialize(ChestObject.ChestTierType.Common);
                    b.hasProp = true;
                    propObjects.Add(chest.gameObject);
                    chest.gameObject.transform.parent = temporaryObjFolder.transform;

                    runtimeDistance = 0;

                    chestsSpawned++;
                    if (chestsSpawned >= numChests)
                    {
                        break;
                    }
                }
            }
        }

        if (M.addSpellbook)
        {
            int distanceCloserToPath = Random.Range(0, 3);
            int distanceToTheBottomLeftOfPortal = Random.Range(4, 9);
            Vector3 spot = pathObjects[pathObjects.Count - 1].gameObject.transform.position + new Vector3(-distanceToTheBottomLeftOfPortal, 0, -distanceCloserToPath);
            Vector3 r = Vector3.down *  180f;

            Block b = GetClosestValidGroundBlock(spot);

            var npc = Instantiate(MapCreator.Instance.blackSmithPrefab, 
                b.transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(r.x, r.y, r.z));
            b.hasProp = true;
            propObjects.Add(npc.gameObject);
            npc.transform.parent = temporaryObjFolder.transform;
        }

        if (M.addMerchant)
        {
            int distanceCloserToPath = Random.Range(0, 3);
            int distanceToTheBottomRightOfPortal = Random.Range(4, 9);

            Vector3 spot = pathObjects[pathObjects.Count - 1].gameObject.transform.position + new Vector3(-distanceCloserToPath, 0, -distanceToTheBottomRightOfPortal);
            Vector3 r = Vector3.down * 90f;

            Block b = GetClosestValidGroundBlock(spot);

            var npc = Instantiate(MapCreator.Instance.merchantPrefab,
                b.transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(r.x, r.y, r.z));
            b.hasProp = true;
            propObjects.Add(npc.gameObject);
            npc.transform.parent = temporaryObjFolder.transform;
        }

        // Ronny, add him only on reward maps, and on any reward map if player still doesn't have a skill
        if (MapCreator.Instance.mapType == MapType.reward && GlobalSettings.Instance.playerCombat.ActiveSkill == null)
        {
            int distanceCloserToPath = Random.Range(0, 3);
            int distanceToTheBottomRightOfPortal = Random.Range(4, 9);

            Vector3 spot = pathObjects[pathObjects.Count - 1].gameObject.transform.position + new Vector3(distanceToTheBottomRightOfPortal, 0, -distanceCloserToPath);
            Vector3 r = Vector3.down * 180f;

            Block b = GetClosestValidGroundBlock(spot);

            var npc = Instantiate(MapCreator.Instance.skillGiverPrefab,
                b.transform.position + new Vector3(0, 0.5f, 0), Quaternion.Euler(r.x, r.y, r.z));
            b.hasProp = true;
            propObjects.Add(npc.gameObject);
            npc.transform.parent = temporaryObjFolder.transform;
        }

    }

    private void AddLegendaryChest(Transform t)
    {
        Block b = GetClosestObject(t.position, allObjects,
                (Block b2) =>
                {
                    return !b2.hasProp && !b2.hasWater;
                });

        var chest = Instantiate(MapCreator.Instance.chestPrefab, b.transform.position + new Vector3(0, 0.5f, 0), GetCameraFacingRotation());
        chest.Initialize(ChestObject.ChestTierType.Legendary);
        b.hasProp = true;
        propObjects.Add(chest.gameObject);
        chest.gameObject.transform.parent = temporaryObjFolder.transform;        
    }

    private void AddEnemies()
    {
        if (!M.addEnemies) return;

        // We don't want enemies to start right from beginning of path
        // because we want player to spawn peacefully and digest the air
        // before encountering something hostile
        int gapOfTilesBeforeFirstEnemy = 14;

        int enemyFrequency = Mathf.Max(1, (pathObjects.Count - gapOfTilesBeforeFirstEnemy) / M.enemySpawnAmount);

        // safety code, if the map is just way too little, ignore the initial gap of enemies
        if (enemyFrequency == 1) enemyFrequency = Mathf.Max(1, (pathObjects.Count) / M.enemySpawnAmount);

        for (int i = gapOfTilesBeforeFirstEnemy; i < pathObjects.Count; ++i)
        {
            if (i % enemyFrequency != 0) continue;
            
            int xOffset = Random.Range(-M.enemySpawnOffset, M.enemySpawnOffset + 1);
            int zOffset = Random.Range(-M.enemySpawnOffset, M.enemySpawnOffset + 1);      

            Vector3 spawnSpot = pathObjects[i].transform.position + new Vector3(xOffset, 0, zOffset);

            GameObject closestPathObject = GetClosestObject(spawnSpot, allObjects, 
                (Block b) =>
                {
                    return !GetGridOccupant(b).hasProp;
                }).gameObject;

            GameObject enemy = Instantiate(M.enemies[Random.Range(0, M.enemies.Length)],
                closestPathObject.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

            enemy.name = "Enemy " + (enemyObjects.Count + 1);
            enemy.transform.parent = enemiesFolder;
            enemyObjects.Add(enemy);
            EnemiesManager.Instance.AddEnemy(enemy);

            MobStats enemyStats = enemy.GetComponentInChildren<MobStats>();
            if (enemyStats)
            {
                float scaleFactor = MapCreator.Instance.enemyScalingPerRound * (GameLoopManager.Instance.nextRoundNumber - 1);

                enemyStats.UpdateAttackDamage(enemyStats.AttackDamage.FinalValue * scaleFactor);
                enemyStats.UpdateAbilityPower(enemyStats.AbilityPower.FinalValue * scaleFactor);
                enemyStats.UpdateMaxHealth(Mathf.CeilToInt(enemyStats.MaxHealth.FinalValue * scaleFactor));
                if (MapCreator.Instance.mapType == MapType.boss || true)
                {
                    enemy.GetComponentInChildren<MobController>().OnDeathEvent += AddLegendaryChest;
                }
            }
        }
    }

    private void AddFoliage()
    {
        int spread = M.foliageDensity;
        
        for (int xLoc = (int)xBoundary.x; xLoc < xBoundary.y; xLoc += spread)
        {
            for (int zLoc = (int)zBoundary.x; zLoc < zBoundary.y; zLoc += spread)
            {
                Vector3 location = new(xLoc, 0, zLoc);
                if (!IsInGrid(location) || null == GetGridOccupant(location).block) continue;

                Block b = GetGridOccupant(location).block;
                SO_BlockItem type = b.type;

                if (Random.Range(0, 1.0f) > b.type.foliageSpawnChance) continue;
                
                for (int x = -1; x < 2; ++x)
                {
                    for (int z = -1; z < 2; ++z)
                    {
                        Vector3 location2 = new Vector3(x, 0, z) + location;
                        if (!IsInGrid(location2) || null == GetGridOccupant(location2).block) continue;
                        GridOccupant gridOccupant = GetGridOccupant(location2);
                        Block b2 = gridOccupant.block;
                        if (b2.hasWater || b2.hasProp) continue;
                        if (gridOccupant.hasProp || gridOccupant.hasWater) continue;
                        if (type != b2.type) continue;

                        GameObject foliage = Instantiate(type.blockFoliage[Random.Range(0, type.blockFoliage.Length)], b2.transform.position, Quaternion.identity);
                        foliage.transform.position += new Vector3(0, 0.5f, 0);
                        foliage.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                        UtilityFunctions.UpdateLODlevels(foliage.transform);
                        propObjects.Add(foliage);
                        foliage.transform.parent = temporaryObjFolder.transform;
                    }
                }                    
            }
        }        
    }

    private void AddProps()
    {
        //// By default, props will spawn in a grid fashion

        // Spread determines how stretched that grid is (spread = 2, means a tree will appear every 2 blocks)
        int spread = M.treeSpawnDensity;

        for (int x = (int)xBoundary.x; x < xBoundary.y; x += spread)
        {
            for (int z = (int)zBoundary.x; z < zBoundary.y; z += spread)
            {
                if (!IsInGrid(new Vector3(x, 0, z)) || null == gridOccupants[x, z].block) continue;

                // Offset: by default, a tree would be placed in its spot in the grid
                // with an offset of 1, the tree could appear 1 block away from the center, 2 would be more
                int randomXOffset = Random.Range(-M.treeSpawnOffset, M.treeSpawnOffset);
                int randomZOffset = Random.Range(-M.treeSpawnOffset, M.treeSpawnOffset);

                int newX = (int)Mathf.Round(x + randomXOffset);
                int newZ = (int)Mathf.Round(z + randomZOffset);

                if (!IsInGrid(new Vector3(newX, 0, newZ))) continue;
                if (null == gridOccupants[newX, newZ].block) continue;
                if (true == gridOccupants[newX, newZ].hasProp) continue;

                // NEW* - don't put normal props on lowest level (where water is, only put water props there)
                if (Mathf.Abs(gridOccupants[newX, newZ].block.transform.position.y - YBoundary.y) < _EPSILON) continue;

                Vector3 spawnSpot = new Vector3(newX, gridOccupants[newX, newZ].block.transform.position.y, newZ);

                // Lastly, distance from path: by default, the chance to spawn a tree on any given block is 0%, but increases by
                // X% for every distance unit away from the closest path block (design choice: spam props around edges of map, to make path clear)
                GameObject closestPathObject = GetClosestObject(spawnSpot, pathObjects).gameObject;
                float chanceOfSpawn = 0.0f;
                chanceOfSpawn += (M.treeChanceGrowthRate * Mathf.Pow(Vector3.Distance(spawnSpot, closestPathObject.transform.position), 1.175f)); //1.35f

                // Actually placing the prop
                if (Random.Range(0f, 100f) < chanceOfSpawn)
                {
                    GameObject obj = Instantiate(M.propSet.propPrefabs[Random.Range(0, M.propSet.propPrefabs.Length)], spawnSpot, Quaternion.identity);
                    UtilityFunctions.UpdateLODlevels(obj.transform);
                    propObjects.Add(obj);
                    obj.transform.position += new Vector3(0, 0.5f, 0); // only 0.5f because the pivot point is center of block
                    obj.transform.parent = temporaryObjFolder.transform;

                    gridOccupants[newX, newZ].hasProp = true;
                }
            }
        }
    }

    private void AddFoundationAndEdgeWork()
    {
        HashSet<Block> blocksAdded = new();
        foreach (Block b in allObjects)
        {
            if (M.addEdgeWalls)
            {
                for (int x = 1; x < 2; ++x)
                {
                    for (int z = 1; z < 2; ++z)
                    {
                        Vector3 areaToCheck = new Vector3(b.transform.position.x + x, 0, b.transform.position.z + z);

                        if (!IsInGrid(areaToCheck) || null == gridOccupants[(int)areaToCheck.x, (int)areaToCheck.z].block)
                        {
                            for (int i = 0; i < M.sizeOfWall; ++i)
                            {
                                GameObject obj = Instantiate(blockPrefab, b.transform.position + new Vector3(0, M.blockRaiseSize + i, 0), Quaternion.identity);

                                obj.transform.parent = temporaryObjFolder.transform;
                                Block block = obj.GetComponent<Block>();
                                block.type = M.blockSet.grass;
                                block.UpdateBlock();
                            }
                        }
                    }
                }
            }

            if (M.addEdgeFoundation)
            {
                for (int x = -1; x < 1; ++x)
                {
                    for (int z = -1; z < 1; ++z)
                    {
                        Vector3 areaToCheck = new Vector3(b.transform.position.x + x, 0, b.transform.position.z + z);

                        if ((!IsInGrid(areaToCheck) || null == gridOccupants[(int)areaToCheck.x, (int)areaToCheck.z].block) && blocksAdded.Add(b))
                        {
                            for (int i = 0; i < 1; ++i)
                            {
                                GameObject obj = Instantiate(blockPrefab, b.transform.position - new Vector3(0, M.blockRaiseSize + i, 0), Quaternion.identity);

                                obj.transform.parent = temporaryObjFolder.transform;
                                Block block = obj.GetComponent<Block>();
                                block.type = M.blockSet.foundation;
                                block.UpdateBlock();

                                blocksAdded.Add(b);
                            }
                        }
                    }
                }
            }
        }        
    }


    // Checks and records latest stats for grid boundary, and max distance, etc.
    private void UpdateBoundaryStats(Vector3 pos)
    {
        if (pos.x < xBoundary.x) xBoundary = new Vector2(pos.x, xBoundary.y);
        if (pos.x > xBoundary.y) xBoundary = new Vector2(xBoundary.x, pos.x);

        if (pos.z < zBoundary.x) zBoundary = new Vector2(pos.z, zBoundary.y);
        if (pos.z > zBoundary.y) zBoundary = new Vector2(zBoundary.x, pos.z);

        // Furthest block from center
        if (allObjects.Count == 0) return;

        Vector3 firstSpawnedObject = allObjects[0].transform.position;
        Vector2 firstSpawnedObject2D = new Vector2(firstSpawnedObject.x, firstSpawnedObject.z);
        float lastSpawnedObjectDistance = Vector2.Distance(firstSpawnedObject2D, pos);

        if (lastSpawnedObjectDistance > Vector2.Distance(firstSpawnedObject2D, furthestBlock))
        {
            furthestBlock = new Vector2(pos.x, pos.z);
            furthestDistance = lastSpawnedObjectDistance;
        }
    }


    private void UpdateLine()
    {
        if (Time.frameCount % 120 != 0) return;

        if (!drawLine)
        {
            line.positionCount = 0;
            return;
        }

        if (allObjects.Count == 0) return;

        Vector3 raiseLevel = new Vector3(0, 0.9f, 0);
        int size = pathObjects.Count;

        line.positionCount = size;
        Vector3[] lineSpots = new Vector3[size];
        Vector3[] lines2 = new Vector3[size];

        for (int i = 0; i < size; i += 1)
        {
            Vector3 goalPosition = pathObjects[i].transform.position;

            if (size > (i + 1) && i > 0 && lineSmoothening)
            {
                Vector3 nextPosition = pathObjects[i + 1].transform.position;
                Vector3 nextNormalized = Vector3.Project(goalPosition, nextPosition);
                Vector3 middleSpot = Vector3.Lerp(goalPosition, nextNormalized, 0.5f);

                Vector3 intermediatePosition = Vector3.Project(goalPosition, middleSpot);

                line.SetPosition(i, intermediatePosition + raiseLevel);
                lineSpots[i] = intermediatePosition + raiseLevel;
                lines2[i] = intermediatePosition + raiseLevel;
            }
            else
            {
                lines2[i] = goalPosition + raiseLevel;
                lineSpots[i] = goalPosition + raiseLevel;
                line.SetPosition(i, goalPosition + raiseLevel);
            }
        }

        if (lineSmoothening)
        {
            for (int i = 1; i < size; i += 2)
            {
                if (i > 0 && i < size - 2)
                    lines2[i] = Vector3.Lerp(lineSpots[i - 1], lineSpots[i + 1], 0.5f);
            }

            lines2[line.positionCount - 1] = lineSpots[size - 1];
        }

        if (lineSmoothening)
        {
            for (int i = 1; i < size; i += 2)
            {
                if (i > 0 && i < size - 2)
                    line.SetPosition(i, Vector3.Lerp(lines2[i - 1], lines2[i + 1], 0.5f));
            }

            line.SetPosition(line.positionCount - 1, lineSpots[size - 1]);
        }
    }


    public GridOccupant GetGridOccupant(Block b)
    {
        return GetGridOccupant(b.gameObject.transform.position);
    }

    public GridOccupant GetGridOccupant(Vector3 v)
    {
        GridOccupant occupant = gridOccupants[(int)v.x, (int)v.z];
        return occupant;
    }

    // Given a source vector, a direction and a scalar, return new vector
    // from source vector in the direction of direction scaled by scalar...
    private Vector3 GetPointOnCircle(Vector3 origin, float radius, float angle)
    {
        float angleInRadians = angle * Mathf.Deg2Rad;
        float x = origin.x + radius * Mathf.Sin(angleInRadians);
        float z = origin.z + radius * Mathf.Cos(angleInRadians);
        return new Vector3(x, origin.y, z);
    }

    private bool IsInGrid(Vector3 spot)
    {
        if ((int)spot.x < 0 || (int)spot.x >= _GRIDSIZE) return false; // can happen if the radius for this lump is bigger than the actual map
        if ((int)spot.z < 0 || (int)spot.z >= _GRIDSIZE) return false;
        return true;
    }

    private bool IsDistanceLessThan(Vector3 a, Vector3 b, float compareValue)
    {
        float dist = (a - b).sqrMagnitude;
        if (dist < (compareValue * compareValue)) return true;
        else return false;
    }

    public Block GetClosestBlock(Transform source)
    {
        return GetClosestObject(source.position, allObjects);
    }

    // Returns the closest block to spot that is valid, doesn't have trees or props, and doesn't have water/liquid
    // NOTE*: this must be used after WaterDips() has already been called since .hasWater is only populated
    // during WaterDips()
    public Block GetClosestValidGroundBlock(Vector3 spot)
    {
        return GetClosestObject(spot, allObjects,
                (Block b2) =>
                {
                    return !b2.hasProp && !b2.hasWater;
                });
    }

    // Get closest valid block to a given vector
    private Block GetClosestObject(Vector3 src, List<Block> list, System.Predicate<Block> condition = null)
    {
        Block closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 srcPosition = src;
        foreach (Block obj in list)
        {
            Vector3 directionToTarget = obj.transform.position - srcPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                if (null == condition || condition(obj))
                {
                    closestDistanceSqr = dSqrToTarget;
                    closest = obj;
                }
            }
        }
        return closest;
    }

    public Quaternion GetCameraFacingRotation(bool reverse = false)
    {
        Vector3 r = Vector3.down * (Random.value < 0.5f ? 90f : 180f);
        if (reverse) r = Vector3.up * (Random.value < 0.5f ? 90f : 180f);
        return Quaternion.Euler(r.x, r.y, r.z);
    }

    public Vector3 GetCameraFacingVector3()
    {
        return Vector3.down * (Random.value < 0.5f ? 90f : 180f);
    }

    public void TeleportPlayer(Vector3 v)
    {
        UtilityFunctions.TeleportObject(GlobalSettings.Instance.player, v);
    }

    private void PartitionProgress(string va = "")
    {
        progressValue++;
        OnProgressChange?.Invoke(progressValue, progressTotal);
        OnProgressChangeText?.Invoke(va);
    }

}
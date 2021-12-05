using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PropItem
{

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
    public struct Blocks
    {
        public SO_BlockItem grass;
        public SO_BlockItem dirt;
        public SO_BlockItem water;
        public SO_BlockItem foundation;
    }

    [System.Serializable]
    public struct ImportantProps
    {
        public GameObject endPoint;        
    }

    [Header("Map Generation Data ")]
    public SO_MapGenData M;
    [Space(10)]

    public GameObject blockPrefab;

    [Header("Block SOs")]
    public Blocks blocks;

    [Header("Important Props")]
    public ImportantProps importantProps;

    public Transform objFolder;
    public GameObject[] propPrefabs;
    public GameObject[] foundationPrefabs;

    [Header("Line (GPS)")]
    public LineRenderer line;
    public bool drawLine = true;
    public bool lineSmoothening = true;
    [Space(10)]

    [Header("Processing Delay")]
    public float processesDelay = 0.02f;

    public static MapGeneration Instance;



    [Space(25)]
    public UnityEvent OnScreenStart = new UnityEvent();
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
    private float pathingAngle;
    private float distanceCreated = 0;
    private GameObject[,] gridOccupants;

    private List<GameObject> allObjects = new List<GameObject>();
    private List<GameObject> pathObjects = new List<GameObject>();
    private List<GameObject> treeObjects = new List<GameObject>();
    private List<GameObject> foundationObjects = new List<GameObject>();
    private List<Block> waterObjects = new List<Block>();
    private List<GameObject> enemyObjects = new List<GameObject>();

    private float progressValue;
    private int progressTotalCounter = 0;
    private float progressTotal = 100f;

    // Need to be initialized
    private Vector2 xBoundary;
    private Vector2 zBoundary;
    private Vector2 yBoundary;
    public Vector2 YBoundary =>
    new Vector2(Mathf.Ceil(M.lumpApplicationRounds / 2.0f),
                Mathf.Ceil(-M.lumpApplicationRounds / 2.0f));

    private Vector2 furthestBlock;
    private float furthestDistance = 0f;

    #endregion


    private void Awake()
    {
        Instance = this;
        _GRIDSIZE = (int)((M.distanceOfPath * 2.0f) + (M.grassFillRadius.y * 2.0f) + 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Can't and shouldn't change these 2 variables mid-play, only one time per play session
        gridOccupants = new GameObject[_GRIDSIZE, _GRIDSIZE];
        centerPoint = new Vector3((int)(_GRIDSIZE / 2), 0, (int)(_GRIDSIZE / 2));
        centerPoint2D = new Vector3(centerPoint.x, centerPoint.z);

        //RegeneratePath();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
        if (Input.GetKeyDown(KeyCode.P)) RegeneratePath();
    }

    public void RegeneratePath() => StartCoroutine(ERegeneratePath());

    public IEnumerator ERegeneratePath()
    {
        yield return null;

        OnScreenStart?.Invoke();

        ResetVariables();

        yield return new WaitForSeconds(0.9f);

        OnMapGenerateStart?.Invoke();

        StartCoroutine(ResetGeneration());
    }

    private void ResetVariables()
    {
        progressValue = -1f;

        xBoundary = centerPoint2D;
        zBoundary = centerPoint2D;
        furthestBlock = centerPoint2D;
        furthestDistance = 0;
        progressTotalCounter = 0;

        distanceCreated = 0;
        pathingAngle = Random.Range(0f, 360f);

        line.positionCount = 0;
        currentPaintingBlock = blocks.grass;
    }

    private IEnumerator ResetGeneration()
    {
        // Resetting object lists
        for (int x = 0; x < _GRIDSIZE; ++x)
        {
            for (int z = 0; z < _GRIDSIZE; ++z)
            {
                if (gridOccupants[x, z] != null)
                {
                    gridOccupants[x, z].gameObject.SetActive(false);
                    gridOccupants[x, z] = null;
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

        enemyObjects.Clear();

        // Important Props



        PartitionProgress("Initiating building engine...");
        yield return new WaitForSeconds(processesDelay);

        /* * * * * CREATION OF MAP * * * * */

        currentPaintingBlock = blocks.grass;

        SpawnPath();
        PartitionProgress("Adding procedural base map...");
        yield return new WaitForSeconds(processesDelay);

        //ThickenPath();
        for (int i = 0; i < pathObjects.Count; i++)
        {
            ThickenAroundObject(pathObjects[i], i, M.grassFillRadius);
            PartitionProgress();
            yield return null;
        }
        PartitionProgress("Expanding area...");
        yield return new WaitForSeconds(processesDelay);

        ThickenAroundObject(pathObjects[pathObjects.Count - 1], 0, M.grassFillRadius);
        PartitionProgress();
        yield return new WaitForSeconds(processesDelay);

        /* * * * * SHAPE MODIFICATION OF MAP * * * * */

        AddRandomLumps();
        PartitionProgress("Modeling beziers...");
        yield return new WaitForSeconds(processesDelay);

        SmoothenElevation();
        PartitionProgress();
        yield return new WaitForSeconds(processesDelay);

        /* * * * * * DECALS ON SHAPE OF MAP * * * * * */

        currentPaintingBlock = blocks.dirt;

        PaintDirtPath();
        PartitionProgress("Shaping map...");
        yield return new WaitForSeconds(processesDelay);

        //ApplyRandomElevation();
        //PartitionProgress("Activating props...");
        //yield return new WaitForSeconds(processesDelay);

        AddFoundationAndEdgeWork();
        PartitionProgress("Finalizing post processing...");
        yield return new WaitForSeconds(processesDelay);

        /* * * * * IMPORTANT PROPS ON MAP * * * * * * */
        AddImportantProps();

        //AddEnemies();

        /* * * * * DECORATIVE PROPS ON MAP * * * * * * */
        currentPaintingBlock = blocks.water;

        AddWaterToDips();
        PartitionProgress("Completed!");
        yield return new WaitForSeconds(processesDelay);

        AddProps();
        PartitionProgress();
        yield return new WaitForSeconds(processesDelay);

        /* * * * MISC STEPS (NO ORDER REQUIRED) * * */



        progressTotal = progressTotalCounter - 1;

        centerPointWithY = new Vector3(centerPoint.x, gridOccupants[(int)centerPoint.x,
            (int)centerPoint.z].transform.position.y, centerPoint.z);

        OnMapGenerateEnd?.Invoke();
        yield return new WaitForSeconds(0.9f);
        OnScreenReady?.Invoke();
    }

    private void SpawnPath()
    {
        int index = 0;
        while (distanceCreated < M.distanceOfPath)
        {
            float randomAngle = Random.Range(M.turningAngleRange.x, M.turningAngleRange.y);

            randomAngle *= ((Random.value > 0.5f) ? 1.0f : -1.0f);

            float newAngle = pathingAngle + randomAngle;

            float randomDistance = Random.Range(M.distanceBeforeTurningPath.x, M.distanceBeforeTurningPath.y);

            if (allObjects.Count == 0)
            {
                GameObject obj = Spawn(centerPoint)?.gameObject;
                if (obj)
                {
                    allObjects.Add(obj);
                    pathObjects.Add(obj);
                }
            }
            else
            {
                SpawnThroughPath(allObjects[allObjects.Count - 1], newAngle, randomDistance);
            }
        }
    }

    private void SpawnThroughPath(GameObject turningPointObj, float newAngle, float scale)
    {
        for (float i = 0.25f; i <= scale; i += 0.25f)
        {
            GameObject newObj = Spawn(GetPointOnCircle(turningPointObj.transform.position, i, newAngle))?.gameObject;

            if (newObj)
            {
                allObjects.Add(newObj);
                pathObjects.Add(newObj);

                pathingAngle = newAngle;

                distanceCreated += Vector3.Distance(pathObjects[pathObjects.Count - 1]
                    .transform.position, pathObjects[pathObjects.Count - 2].transform.position);
            }
        }
    }

    // From a center object, this loops through all 360 angles, 20 at a
    // time, and draws objects to fill the circle
    private void ThickenPath()
    {
        for (int i = 0; i < pathObjects.Count; i++)
        {
            ThickenAroundObject(pathObjects[i], i, M.grassFillRadius);
        }
    }

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
        int randomizingCap = (int)(System.Math.Min(fillRadius.x - 1, 2));
        int randomizedX = Random.Range(-randomizingCap, randomizingCap);
        int randomizedZ = Random.Range(-randomizingCap, randomizingCap);
        Vector3 centerObjectOffsetted = obj.transform.position + new Vector3(randomizedX, 0, randomizedZ);

        // Looping through all areas in the circle, and spawning another block
        for (float x = -thickness; x < thickness; x += 1f)
        {
            for (float z = -thickness; z < thickness; z += 1f)
            {
                Vector3 newSpot = centerObjectOffsetted + new Vector3((int)x, 0, (int)z);

                if (IsDistanceLessThan(centerObjectOffsetted, newSpot, (thickness * M.circularity))) // tags: circular
                {
                    Block replacedBlock = null;
                    Block block = SpawnAdvanced(newSpot, ref replacedBlock);

                    if (block) allObjects.Add(block.gameObject);

                    // This part here is because we have a special condition to apply to dirt paths
                    // which is to never have them on the lowest elevation layer, and if they are, elevate them
                    // (design choice: don't want dirt to be covered with water - dirt paths should always be clear to walk on)
                    if (null != replacedBlock && replacedBlock.type == blocks.dirt)
                    {
                        ApplyBlockElevationRestrictions(replacedBlock);
                    }
                }
            }
        }
    }

    private void ApplyBlockElevationRestrictions(Block block)
    {
        GameObject newObj = block.gameObject;

        if (Mathf.Abs(Mathf.Round(newObj.transform.position.y) - YBoundary.y) < _EPSILON)
        {
            ElevateBlock(true, newObj);
        }
    }

    private void AddRandomLumps()
    {
        //// By default, lumps will spawn in a grid fashion
        // Spread determines how far lumps are apart (spread = 2, means a tree will appear every 2 blocks)
        int spread = M.lumpDensity;

        for (int upOrDown = 0; upOrDown < M.lumpApplicationRounds; ++upOrDown)
        {
            for (int x = (int)xBoundary.x; x < xBoundary.y; x += spread)
            {
                for (int z = (int)zBoundary.x; z < zBoundary.y; z += spread)
                {
                    if (null == gridOccupants[x, z]) continue;

                    // Any other conditions
                    GameObject firstSpawnedBlock = allObjects[0];
                    GameObject checkingBlock = gridOccupants[x, z];
                    if (checkingBlock == null) continue;

                    // if we're currently adding the top-side lumps, and this object is on bottom side of block, then continue
                    if (upOrDown % 2 == 0 && IsOnSideOfBlock(Side.south, firstSpawnedBlock, checkingBlock))
                    {
                        //continue;
                    }
                    else if (upOrDown % 2 != 0 && IsOnSideOfBlock(Side.north, firstSpawnedBlock, checkingBlock))
                    {
                        //continue;
                    }
                    ///////////////////////

                    int randomXOffset = Random.Range(-M.lumpOffset, M.lumpOffset + 1);
                    int randomZOffset = Random.Range(-M.lumpOffset, M.lumpOffset + 1);

                    x += randomXOffset;
                    z += randomZOffset;

                    ElevateCircle(upOrDown % 2 != 0 ? true : false, new Vector3(x, 0, z), Random.Range(M.lumpRadius.x, M.lumpRadius.y));
                }
            }
        }
    }

    private void ElevateCircle(bool upOrDown, Vector3 area, float radius)
    {
        List<GameObject> alreadyElevated = new List<GameObject>();

        // Looping through all areas in the circle, and spawning another block
        for (float x = -radius; x < radius; x += 1f)
        {
            for (float z = -radius; z < radius; z += 1f)
            {
                Vector3 newSpot = area + new Vector3(x, 0, z);

                //if (Vector3.Distance(area, newSpot) < (radius* M.circularity)) // tags: circular
                if (IsDistanceLessThan(area, newSpot, (radius * M.circularity)))
                {
                    GameObject potentialObj = gridOccupants[(int)newSpot.x, (int)newSpot.z];

                    if (potentialObj != null && !alreadyElevated.Contains(potentialObj))
                    {
                        ElevateBlock(upOrDown, potentialObj);
                    }
                }
            }
        }
        alreadyElevated.Clear();
    }

    private void ElevateBlock(bool upOrDown, GameObject obj)
    {
        float extremeY;
        extremeY = GetAdjacentElevation(upOrDown ? ElevationLevel.lowest : ElevationLevel.highest, obj);

        float yValue = extremeY + (upOrDown ? 1 : -1);
        yValue = Mathf.Clamp(yValue, YBoundary.y, YBoundary.x);

        obj.transform.position = new Vector3(obj.transform.position.x, yValue, obj.transform.position.z);

        Block block = obj.GetComponentInChildren<Block>();
        if (block) block.UpdateHistory((upOrDown ? "Raised" : "Lowered") + " to " + obj.transform.position);
    }

    private Block Spawn(Vector3 vec, bool utilizeY = false)
    {
        Block replacementBlock = null;
        return SpawnAdvanced(vec, ref replacementBlock, utilizeY);
    }

    private Block SpawnAdvanced(Vector3 vec, ref Block replacedBlock, bool utilizeY = false)
    {
        if (!utilizeY) // majority of the time you use Spawn(), you ignore y, cause you're affecting grid[x,z]
        {
            GameObject objectAtVec = gridOccupants[(int)vec.x, (int)vec.z];
            if (null != objectAtVec)
            {
                Block blockAtVec = objectAtVec.GetComponentInChildren<Block>();
                replacedBlock = blockAtVec;
                blockAtVec.type = currentPaintingBlock;
                blockAtVec.UpdateBlock();
                return null;
            }
        }

        Vector3 spawnSpot = new Vector3((int)vec.x, (int)vec.y, (int)vec.z);

        GameObject obj = Pool.Instance.Instantiate(blockPrefab.name, spawnSpot, Quaternion.identity);

        Block block = obj.GetComponentInChildren<Block>();
        block.type = currentPaintingBlock;
        block.UpdateBlock();
        block.UpdateHistory("Spawned at " + spawnSpot);

        if (utilizeY) return block;

        gridOccupants[(int)vec.x, (int)vec.z] = obj;

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

    private float GetAdjacentElevation(ElevationLevel level, GameObject src)
    {
        float extremestY = src.transform.position.y;

        for (int x = -1; x < 2; ++x)
        {
            for (int z = -1; z < 2; ++z)
            {
                Vector3 areaToCheck = new Vector3(src.transform.position.x + x, 0, src.transform.position.z + z);
                GameObject objectToCheck = gridOccupants[(int)areaToCheck.x, (int)areaToCheck.z];
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

    private int IsAdjacentOccupied(Vector3 src, bool sameElevationNeeded = false)
    {
        int sidesCovered = 0;
        for (int x = -1; x < 2; ++x)
        {
            for (int z = -1; z < 2; ++z)
            {
                Vector3 areaToCheck = new Vector3(src.x + x, 0, src.z + z);
                GameObject objectToCheck = gridOccupants[(int)areaToCheck.x, (int)areaToCheck.z];

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

    private void PaintDirtPath()
    {
        for (int i = 0; i < pathObjects.Count; i++)
        {
            // Cuts dirt path - if a cut is too happen (frequency met), then skip i to dirtCutLength
            if (i % M.dirtCutoffFrequency == 0)
            {
                int dirtCutLength = (int)Random.Range(M.dirtCutoffLength.x, M.dirtCutoffLength.y);
                i += dirtCutLength;
                if (i >= pathObjects.Count) return;
            }

            pathObjects[i].GetComponent<Block>().type = blocks.dirt;
            ThickenAroundObject(pathObjects[i], i, M.dirtFillRadius);
        }
    }

    // Goes through all ground blocks and raises/lowers their y-value by a small amount
    // to give that blocky/cubica look
    // THIS NEEDS TO HAPPEN LAST (in steps of Map Generation)
    private void ApplyRandomElevation()
    {
        for (int i = 0; i < allObjects.Count; i++)
        {
            //if (allObjects[i].GetComponentInChildren<Block>().type != blocks.dirt) continue;

            float elevation = Random.Range(M.randomElevation.x, M.randomElevation.y);
            float xRandom = 0; // Random.Range(-M.randomElevation.x / 2f, M.randomElevation.x / 2f);
            float zRandom = 0; // Random.Range(-M.randomElevation.x / 2f, M.randomElevation.x / 2f);

            allObjects[i].transform.position += new Vector3(xRandom, elevation, zRandom);
        }
    }

    private void AddWaterToDips()
    {
        float yLevelToMeasure = Mathf.Round(YBoundary.y);

        for (int i = 0; i < allObjects.Count; ++i)
        {
            Transform thisObject = allObjects[i].transform;

            if (Mathf.Abs((Mathf.Round(thisObject.position.y)) - yLevelToMeasure) < _EPSILON)
            {
                Vector3 spawnSpot = new Vector3(thisObject.position.x, yLevelToMeasure + 1, thisObject.transform.position.z);

                GameObject waterObj = Pool.Instance.Instantiate(blockPrefab.name, spawnSpot, Quaternion.identity);
                if (waterObj)
                {
                    Block waterBlock = waterObj.GetComponent<Block>();
                    waterBlock.type = blocks.water;
                    waterBlock.UpdateBlock();
                    waterObjects.Add(waterBlock);
                }
            }
        }
    }

    private void AddImportantProps()
    {
        // EndPoint
        Vector3 spawnSpot = pathObjects[pathObjects.Count - 1].transform.position + new Vector3(0, 1, 0);
        GameObject obj = Instantiate(importantProps.endPoint, spawnSpot, Quaternion.identity);
        treeObjects.Add(obj);        
    }

    private void AddEnemies()
    {
        for (int i = 1; i < pathObjects.Count; ++i)
        {
            if (i % M.enemyFrequency != 0) continue;            

            int amountToSpawn = 1;
            if (i % M.spawnDoubleEveryFrequency == 0) amountToSpawn = 2;

            for (int amount = 0; amount < amountToSpawn; ++amount)
            {
                int xOffset = Random.Range(-M.enemySpawnOffset, M.enemySpawnOffset + 1);
                int zOffset = Random.Range(-M.enemySpawnOffset, M.enemySpawnOffset + 1);

                Vector3 spawnSpot = pathObjects[i].transform.position + new Vector3(xOffset, 5f, zOffset);

                GameObject enemy = Pool.Instance.Instantiate(M.enemies[Random.Range(0, M.enemies.Length)].name, spawnSpot, Quaternion.identity);
                if (enemy)
                {
                    enemyObjects.Add(enemy);
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
                if (null == gridOccupants[x, z]) continue;

                // Offset: by default, a tree would be placed in its spot in the grid
                // with an offset of 1, the tree could appear 1 block away from the center, 2 would be more
                int randomXOffset = Random.Range(-M.treeSpawnOffset, M.treeSpawnOffset);
                int randomZOffset = Random.Range(-M.treeSpawnOffset, M.treeSpawnOffset);

                int newX = (int)Mathf.Round(x + randomXOffset);
                int newZ = (int)Mathf.Round(z + randomZOffset);

                if (null == gridOccupants[newX, newZ]) continue;

                // NEW* - don't put normal props on lowest level (where water is, only put water props there)
                float yLevelToMeasure = Mathf.Round(YBoundary.y);
                if (Mathf.Abs((Mathf.Round(gridOccupants[newX, newZ].transform.position.y)) - yLevelToMeasure) < _EPSILON) continue;

                Vector3 spawnSpot = new Vector3(newX, gridOccupants[newX, newZ].transform.position.y, newZ);

                // Lastly, distance from path: by default, the chance to spawn a tree is 0%, but increases by
                // 5% for every distance unit away from the closest path block 
                GameObject closestPathPosition = GetClosestObject(spawnSpot, pathObjects);

                float chanceOfSpawn = 0.0f;
                chanceOfSpawn += (M.treeChanceGrowthRate * Mathf.Pow(Vector3.Distance(spawnSpot, closestPathPosition.transform.position), 1.15f)); //1.35f

                if (Random.Range(0f, 100f) < chanceOfSpawn)
                {
                    GameObject obj = Instantiate(propPrefabs[Random.Range(0, propPrefabs.Length)], spawnSpot, Quaternion.identity);
                    treeObjects.Add(obj);
                    obj.transform.position += new Vector3(0, 0.5f, 0);
                    obj.transform.SetParent(objFolder);
                }
            }
        }
    }

    private void SmoothenElevation()
    {
        for (int i = 0; i < allObjects.Count; ++i)
        {
            GameObject obj = allObjects[i];
            if (IsAdjacentOccupied(obj.transform.position, true) < M.flattenIfSurroundedByLessThan)
            {
                if (Mathf.Abs(obj.transform.position.y - YBoundary.x) < Mathf.Abs(obj.transform.position.y - YBoundary.y))
                {
                    ElevateBlock(false, obj);
                }
                else
                {
                    ElevateBlock(true, obj);
                }
            }
        }
    }

    private void AddFoundationAndEdgeWork()
    {
        int size = allObjects.Count;
        for (int i = 0; i < size; ++i)
        {
            if (9 == IsAdjacentOccupied(allObjects[i].gameObject.transform.position)) continue;

            // Adding foundation layer blocks underneath map
            //int yLevel = (int)(allObjects[i].transform.position.y);

            //GameObject foundationBlock = Pool.Instance.Instantiate(foundationPrefabs[Random.Range(0, foundationPrefabs.Length)].name,
            //    new Vector3(allObjects[i].transform.position.x,
            //    yLevel - 0.5f, allObjects[i].transform.position.z), Quaternion.identity);

            //if (foundationBlock) foundationObjects.Add(foundationBlock);

            // RAISING edge blocks (so that anything, ie. water blocks, don't look awkward at edges)
            Vector3 objPos = allObjects[i].transform.position;
            GameObject obj = gridOccupants[(int)objPos.x, (int)objPos.z];
            if (null != obj && Mathf.Abs(objPos.y - YBoundary.y) < _EPSILON)
            {
                ElevateBlock(true, obj);
            }

        }
    }


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

    private float Rounded(float v)
    {
        return (((int)(v * 100.0f)) / 100.0f);
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

    private bool IsDistanceLessThan(Vector3 a, Vector3 b, float compareValue)
    {
        float dist = (a - b).sqrMagnitude;
        if (dist < (compareValue * compareValue)) return true;
        else return false;
    }

    private GameObject GetClosestObject(Vector3 src, List<GameObject> list)
    {
        GameObject closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 srcPosition = src;
        foreach (GameObject obj in list)
        {
            Vector3 directionToTarget = obj.transform.position - srcPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closest = obj;
            }
        }
        return closest;
    }

    private void PartitionProgress(string va = "")
    {
        progressTotalCounter++;
        progressValue++;
        OnProgressChange?.Invoke(progressValue, progressTotal);
        OnProgressChangeText?.Invoke(va);
    }

    public void RenderBlocks() => StartCoroutine(ERenderBlocks());
    public IEnumerator ERenderBlocks()
    {
        for (int i = 0; i < allObjects.Count; ++i)
        {
            allObjects[i].GetComponent<Block>().ToggleRenderer(true); // UpdateBlockItem();
            yield return null;
        }
        for (int i = 0; i < waterObjects.Count; ++i)
        {
            waterObjects[i].GetComponent<Block>().ToggleRenderer(true); //    UpdateBlockItem();
            yield return null;
        }
    }


}

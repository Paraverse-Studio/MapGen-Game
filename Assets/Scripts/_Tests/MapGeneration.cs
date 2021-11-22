using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    [System.Serializable]
    public struct Blocks
    {
        public TestBlockType grass;
        public TestBlockType dirt;
        public TestBlockType treeTrunk;
        public TestBlockType treeLeaves;
        public TestBlockType props;
    }
    public Blocks blocks;
    public Transform objFolder;
    public GameObject blockPrefab;
    public GameObject[] treePrefabs;
    public LineRenderer line;
    public bool drawLine = true;

    [Space(5)]
    [Header("    ———————   BASE GRID   ———————")]
    [Space(15)]
    public float randomElevation;

    [Space(5)]
    [Header("   PATH SIZE ")]
    public float distanceOfPath = 40f;

    [Space(5)]
    [Header("   PATH TWISTING ")]
    [MinMaxSlider(0f, 30f)]
    public Vector2 distanceBeforeTurningPath;
    public float turningAngleMax;

    [Space(5)]
    [Header("   PATH THICKNESS ")]
    public int thickenFrequency = 8;

    [MinMaxSlider(0f, 15f)]
    public Vector2 grassFillRadius;

    [MinMaxSlider(0f, 15f)]
    public Vector2 dirtFillRadius;

    [Space(5)]
    [Header("   LUMP DENSITY ")]
    public int lumpDensity = 10;

    [Header("   LUMP RADIUS ")]
    [MinMaxSlider(0f, 9f)]
    public Vector2 lumpRadius;

    [Space(5)]
    [Header("   LUMP-OFFSET ")]
    public int lumpOffset = 2;

    [Space(5)]
    [Header("   DIRT CUT-OFF ")]
    public int dirtCutoffFrequency;

    [MinMaxSlider(0f, 12f)]
    public Vector2 dirtCutoffLength;

    [Space(5)]
    [Header("    ———————   MAP PROPS   ———————")]
    [Space(15)]
    public bool showProps;

    [Space(5)]
    [Header("   TREE DENSITY ")]
    public int treeDensity = 4;

    [Space(5)]
    [Header("   SPAWN-OFFSET ")]
    public int treeOffset = 2;

    [Space(5)]
    [Header("   SPAWN CHANCE GROWTH ")]
    public float treeChanceGrowthRate = 5.0f;


    [Space(20)]

    #region SETTINGS_VARIABLES
    private int gridSize = 1000;

    private TestBlockType currentPaintingBlock;
    #endregion

    #region RUNTIME_VARIABLES
    private float pathingAngle;
    private float distanceCreated = 0;
    private GameObject[,] gridOccupied;

    private List<GameObject> allObjects = new List<GameObject>();
    private List<GameObject> pathObjects = new List<GameObject>();
    private List<GameObject> treeObjects = new List<GameObject>();

    private Vector2 xBoundary;
    private Vector2 zBoundary;
    private Vector2 furthestBlock = Vector2.zero;
    private float furthestDistance = 0f;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        ResetGeneration();

        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
    }


    private void GenerateMap()
    {
        // Grass base series
        currentPaintingBlock = blocks.grass;

        SpawnPath();

        ThickenPath();

        ThickenAroundObject(pathObjects[pathObjects.Count - 1], 0, grassFillRadius);

        AddRandomLumps();

        //Dirt series
        currentPaintingBlock = blocks.dirt;

        //PaintDirtPath();

        //ApplyRandomElevation();

        //AddProps();
    }

    [Button]
    private void RegeneratePath()
    {
        ResetGeneration();

        GenerateMap();
    }

    private void ResetGeneration()
    {
        distanceCreated = 0;
        pathingAngle = Random.Range(0f, 360f);

        gridOccupied = new GameObject[1000, 1000];
        for (int i = 0; i < 1000; ++i) gridOccupied[i, i] = null;

        line.positionCount = 0;

        currentPaintingBlock = blocks.grass;

        if (allObjects.Count > 0)
        {
            for (int i = allObjects.Count - 1; i >= 0; --i) Destroy(allObjects[i]);
        }
        if (pathObjects.Count > 0)
        {
            for (int i = pathObjects.Count - 1; i >= 0; --i) Destroy(pathObjects[i]);
        }
        if (treeObjects.Count > 0)
        {
            for (int i = treeObjects.Count - 1; i >= 0; --i) Destroy(treeObjects[i]);
        }

        pathObjects = new List<GameObject>();
        allObjects = new List<GameObject>();
        treeObjects = new List<GameObject>();
    }


    private void SpawnPath()
    {
        while (distanceCreated < distanceOfPath)
        {
            float randomAngle = Random.Range(-turningAngleMax, turningAngleMax);
            float newAngle = pathingAngle + randomAngle;

            float randomDistance = Random.Range(distanceBeforeTurningPath.x, distanceBeforeTurningPath.y);

            if (allObjects.Count == 0)
            {
                GameObject obj = Spawn(Vector3.zero);
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
            GameObject newObj = Spawn(GetPointOnCircle(turningPointObj.transform.position, i, newAngle));

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
            ThickenAroundObject(pathObjects[i], i, grassFillRadius);
        }
    }

    private void ThickenAroundObject(GameObject obj, int i, Vector2 fillRadius)
    {
        // Determining what type of circle fill
        float thickness;
        if (i % thickenFrequency != 0)
        {
            thickness = fillRadius.x;
        }
        else
        {
            thickness = Random.Range(fillRadius.x, fillRadius.y);
        }

        // Randomizing the circle radius' off-set a bit
        int randomizingCap = (int)(Mathf.Min(fillRadius.x - 1, 2));
        int randomizedX = Random.Range(-randomizingCap, randomizingCap);
        int randomizedZ = Random.Range(-randomizingCap, randomizingCap);
        Vector3 centerObjectOffsetted = obj.transform.position + new Vector3(randomizedX, 0, randomizedZ);

        // Looping through all areas in the circle, and spawning another block
        for (float x = -thickness; x < thickness; x += 0.5f)
        {
            for (float z = -thickness; z < thickness; z += 0.5f)
            {
                Vector3 newSpot = centerObjectOffsetted + new Vector3(x, 0, z);
                if (Vector3.Distance(centerObjectOffsetted, newSpot) < thickness)
                {
                    GameObject newObj = Spawn(newSpot);
                    if (newObj)
                    {
                        allObjects.Add(newObj);
                    }
                }
            }
        }
    }

    private void RandomizePathElevation()
    {
        int size = allObjects.Count;
        float currentElevation = 0f;

        bool alreadyElevated = false;

        for (int i = 0; i < (int)furthestDistance; i += 5)
        {
            // Randomize slight elevation
            for (int index = 0; index < size; ++index)
            {
                if (Vector3.Distance(allObjects[0].transform.position, allObjects[index].transform.position) >= i)
                {
                    GameObject firstSpawnedBlock = allObjects[0];
                    GameObject latestSpawnedBlock = allObjects[index];
                    if (latestSpawnedBlock.transform.position.x < firstSpawnedBlock.transform.position.x ||
                        latestSpawnedBlock.transform.position.z > firstSpawnedBlock.transform.position.z)
                    {
                        //if (!alreadyElevated)
                        //{
                        //    currentElevation = latestSpawnedBlock.transform.position.y + 1;
                        //    alreadyElevated = true;
                        //}
                        //latestSpawnedBlock.transform.position = new Vector3(latestSpawnedBlock.transform.position.x, currentElevation, latestSpawnedBlock.transform.position.z);

                        float lowestAround = GetAdjacentLowest(allObjects[index]);
                        latestSpawnedBlock.transform.position = new Vector3(latestSpawnedBlock.transform.position.x, lowestAround + 1f, latestSpawnedBlock.transform.position.z);
                    }
                }
            }
            alreadyElevated = false;
        }

        alreadyElevated = false;

        for (int i = 0; i < (int)furthestDistance; i += 5)
        {
            // Randomize slight elevation
            for (int index = 0; index < size; ++index)
            {
                if (Vector3.Distance(allObjects[0].transform.position, allObjects[index].transform.position) >= i)
                {
                    GameObject firstSpawnedBlock = allObjects[0];
                    GameObject latestSpawnedBlock = allObjects[index];
                    if (latestSpawnedBlock.transform.position.x > firstSpawnedBlock.transform.position.x &&
                        latestSpawnedBlock.transform.position.z < firstSpawnedBlock.transform.position.z)
                    {
                        float highestAround = GetAdjacentHighest(allObjects[index]);
                        latestSpawnedBlock.transform.position = new Vector3(latestSpawnedBlock.transform.position.x, highestAround - 1f, latestSpawnedBlock.transform.position.z);
                    }
                }
            }
            alreadyElevated = false;
        }
    }

    private void AddRandomLumps()
    {
        //// By default, lumps will spawn in a grid fashion

        // Spread determines how far lumps are apart (spread = 2, means a tree will appear every 2 blocks)
        int spread = lumpDensity;

        for (int upOrDown = 0; upOrDown < 4; ++upOrDown)
        {
            for (int x = (int)xBoundary.x; x < xBoundary.y; x += spread)
            {
                for (int z = (int)zBoundary.x; z < zBoundary.y; z += spread)
                {
                    int xShifted = x + (int)(gridSize / 2);
                    int zShifted = z + (int)(gridSize / 2);
                    if (null == gridOccupied[xShifted, zShifted]) continue;

                    // Any other conditions
                    GameObject firstSpawnedBlock = allObjects[0];
                    GameObject latestSpawnedBlock = gridOccupied[xShifted, zShifted];

                    // if we're currently adding the top-side lumps, and this object is on bottom side of block, then continue
                    if (upOrDown % 2 == 0 && IsOnSideOfBlock(false, firstSpawnedBlock, latestSpawnedBlock))
                    {
                        continue;
                    }
                    else if (upOrDown % 2 != 0 && IsOnSideOfBlock(true, firstSpawnedBlock, latestSpawnedBlock))
                    {
                        continue;
                    }
                    ///////////////////////

                    int randomXOffset = Random.Range(-lumpOffset, lumpOffset + 1);
                    int randomZOffset = Random.Range(-lumpOffset, lumpOffset + 1);

                    x += randomXOffset;
                    z += randomZOffset;

                    ElevateCircle(upOrDown % 2 == 0 ? true : false, new Vector3(x, 0, z), Random.Range(lumpRadius.x, lumpRadius.y));
                }
            }
        }
    }

    private void ElevateCircle(bool upOrDown, Vector3 area, float radius)
    {
        List<GameObject> alreadyElevated = new List<GameObject>();

        // Looping through all areas in the circle, and spawning another block
        for (float x = -radius; x < radius; x += 0.5f)
        {
            for (float z = -radius; z < radius; z += 0.5f)
            {
                Vector3 newSpot = area + new Vector3(x, 0, z);

                if (Vector3.Distance(area, newSpot) < radius)
                {
                    Vector2 gridPos = GetGridCoordsOfBlock(newSpot);
                    GameObject potentialObj = gridOccupied[(int)gridPos.x, (int)gridPos.y];
                    if (potentialObj != null && !alreadyElevated.Contains(potentialObj))
                    {
                        alreadyElevated.Add(potentialObj);
                        float extremeY;
                        if (upOrDown) extremeY = GetAdjacentLowest(potentialObj);
                        else extremeY = GetAdjacentHighest(potentialObj);

                        potentialObj.transform.position += new Vector3(0, extremeY + (upOrDown ? 0.5f : -0.5f), 0);
                    }
                }
            }
        }
        alreadyElevated.Clear();
    }

    private GameObject Spawn(Vector3 vec, bool replace = false)
    {
        Vector3 simplePosition = new Vector3(Mathf.Round(vec.x), vec.y, Mathf.Round(vec.z));

        int x = (int)simplePosition.x + (int)(gridSize / 2);
        int z = (int)simplePosition.z + (int)(gridSize / 2);

        if (null != gridOccupied[x, z] && gridOccupied[x, z].transform.position.y == vec.y)
        {
            gridOccupied[x, z].GetComponent<TestBlock>().type = currentPaintingBlock;
            return null;
        }

        Vector3 gridPosition = new Vector3(Mathf.Round(simplePosition.x) + 0.5f,
        Mathf.Round(simplePosition.y) + 0.5f, Mathf.Round(simplePosition.z) + 0.5f);

        GameObject obj = Instantiate(blockPrefab, gridPosition, Quaternion.identity);
        obj.GetComponent<TestBlock>().type = currentPaintingBlock;
        obj.transform.parent = objFolder;
        gridOccupied[x, z] = obj;

        UpdateBoundaries(obj.transform.position);

        return obj;
    }

    private void UpdateBoundaries(Vector3 pos)
    {
        if (pos.x < xBoundary.x) xBoundary = new Vector2(pos.x, xBoundary.y);
        if (pos.x > xBoundary.y) xBoundary = new Vector2(xBoundary.x, pos.x);

        if (pos.z < zBoundary.x) zBoundary = new Vector2(pos.z, zBoundary.y);
        if (pos.z > zBoundary.y) zBoundary = new Vector2(zBoundary.x, pos.z);

        if (allObjects.Count == 0) return;
        Vector3 firstSpawnedObject = allObjects[0].transform.position;
        float lastSpawnedObjectDistance = Vector2.Distance(firstSpawnedObject, pos);
        if (lastSpawnedObjectDistance >
            Vector2.Distance(firstSpawnedObject, furthestBlock))
        {
            furthestBlock = new Vector2(pos.x, pos.z);
            furthestDistance = lastSpawnedObjectDistance;
        }
    }

    private Vector2 GetGridCoordsOfBlock(Vector3 obj)
    {
        int x = (int)(obj.x - 0.5f);
        int z = (int)(obj.z - 0.5f);

        x += (int)(gridSize / 2);
        z += (int)(gridSize / 2);

        return new Vector2(x, z);
    }

    private bool IsOnSideOfBlock(bool northOrSouth, GameObject src, GameObject subject)
    {
        Vector3 angle45 = new Vector3(-1, 0, 1);
        Vector3 angleToSubject = subject.transform.position - src.transform.position;

        float thisAngle = Vector3.Angle(angle45, angleToSubject);
        return (northOrSouth ? thisAngle <= 90f : thisAngle > 90f);
    }

    private float GetAdjacentHighest(GameObject src)
    {
        float highest = src.transform.position.y;
        int xShifted = (int)(src.transform.position.x - 0.5f) + (int)(gridSize / 2);
        int zShifted = (int)(src.transform.position.z - 0.5f) + (int)(gridSize / 2);

        for (int x = -1; x < 2; ++x)
        {
            for (int z = -1; z < 2; ++z)
            {
                GameObject objectToCheck = gridOccupied[xShifted + x, zShifted + z];
                if (objectToCheck != null)
                {
                    if (objectToCheck.transform.position.y > highest) highest = objectToCheck.transform.position.y;
                }
            }
        }
        return highest;
    }

    private float GetAdjacentLowest(GameObject src)
    {
        float lowest = src.transform.position.y;
        int xShifted = (int)(src.transform.position.x - 0.5f) + (int)(gridSize / 2);
        int zShifted = (int)(src.transform.position.z - 0.5f) + (int)(gridSize / 2);

        for (int x = -1; x < 2; ++x)
        {
            for (int z = -1; z < 2; ++z)
            {
                GameObject objectToCheck = gridOccupied[xShifted + x, zShifted + z];
                if (objectToCheck != null)
                {
                    if (objectToCheck.transform.position.y < lowest) lowest = objectToCheck.transform.position.y;
                }
            }
        }
        return lowest;
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

    private void UpdateLine()
    {
        if (!drawLine)
        {
            line.positionCount = 0;
            return;
        }

        line.positionCount = pathObjects.Count;

        for (int i = 0; i < pathObjects.Count; i++)
        {
            line.SetPosition(i, pathObjects[i].transform.position + new Vector3(0, 0.6f, 0));
        }
    }


    private void PaintDirtPath()
    {
        for (int i = 0; i < pathObjects.Count; i++)
        {
            if (i % dirtCutoffFrequency == 0)
            {
                int dirtCutLength = (int)Random.Range(dirtCutoffLength.x, dirtCutoffLength.y);
                i += dirtCutLength;
                if (i >= pathObjects.Count) return;
            }

            pathObjects[i].GetComponent<TestBlock>().type = blocks.dirt;
            ThickenAroundObject(pathObjects[i], i, dirtFillRadius);
        }
    }

    // Goes through all ground blocks and raises/lowers their y-value by a small amount
    // to give that blocky/cubica look
    private void ApplyRandomElevation()
    {
        for (int i = 0; i < allObjects.Count; i++)
        {
            float elevation = Random.Range(-randomElevation, randomElevation);
            allObjects[i].transform.position += new Vector3(0,
                allObjects[i].transform.position.y + elevation, 0);
        }
    }

    private void AddProps()
    {
        //// By default, props will spawn in a grid fashion

        // Spread determines how stretched that grid is (spread = 2, means a tree will appear every 2 blocks)
        int spread = treeDensity;

        for (int x = (int)xBoundary.x; x < xBoundary.y; x += spread)
        {
            for (int z = (int)zBoundary.x; z < zBoundary.y; z += spread)
            {
                int xShifted = x + (int)(gridSize / 2);
                int zShifted = z + (int)(gridSize / 2);
                if (null == gridOccupied[xShifted, zShifted]) continue;

                // Offset: by default, a tree would be placed in its spot in the grid
                // with an offset of 1, the tree could appear 1 block away from the center, 2 would be more
                int randomXOffset = Random.Range(-treeOffset, treeOffset);
                int randomZOffset = Random.Range(-treeOffset, treeOffset);

                Vector3 spawnSpot = new Vector3(
                    Mathf.Round(x) + 0.5f + randomXOffset,
                    gridOccupied[xShifted, zShifted].transform.position.y,
                    Mathf.Round(z) + 0.5f + randomZOffset);

                // Lastly, distance from path: by default, the chance to spawn a tree is 0%, but increases by
                // 5% for every distance unit away from the closest path block 
                GameObject closestPathPosition = GetClosestObject(spawnSpot, pathObjects);

                float chanceOfSpawn = -10.0f;
                chanceOfSpawn += (treeChanceGrowthRate * Mathf.Pow(Vector3.Distance(spawnSpot, closestPathPosition.transform.position), 1.35f));

                if (Random.Range(0f, 100f) < chanceOfSpawn)
                {
                    GameObject obj = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], spawnSpot, Quaternion.identity);
                    treeObjects.Add(obj);
                    obj.transform.position += new Vector3(0, 1, 0);
                    obj.transform.parent = objFolder;

                    if (GlobalSettings.Instance.showHudText || true)
                    {
                        obj.GetComponentInChildren<TestBlock>().TextToDisplay =
                            Rounded(Vector3.Distance(spawnSpot, closestPathPosition.transform.position)) + " (" + Rounded(chanceOfSpawn) + "%)";
                    }
                }
            }
        }
    }


}

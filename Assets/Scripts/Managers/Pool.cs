using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPoolItem
{
    void Restart();
}


[System.Serializable]
public class PoolItem  // a single instance of "List<ObjectPoolItem> itemsToPool" means all this info
{
    public GameObject objectToPool;
    public GameObject parentObj;
    public GameObject customFolder;
    public int amount;
    public bool doSpawn = true;
    public bool canExpand;
    public string notes;
}

public class Pool : MonoBehaviour
{
    public int totalSpawned = 0;
    public int totalProgress = 0;
    public float scale = 1;

    private int maxSize = 1;

    public int delayAfterEverySet = 1000;
    private int delaySetCounter = 0;

    public Transform folder;
    public Transform waterVolume;
    // list that describes each object pooled object
    public List<PoolItem> itemsToPool;

    // list that holds all pooledobject lists
    //public List<GameObject> pooledObjects;
    public GameObject[,] pooledObjects;

    [Space(20)]
    public UnityEvent OnPoolCreateStart = new UnityEvent();
    public UnityEvent OnPoolCreateEnd = new UnityEvent();

    public FloatFloatEvent OnProgressChange = new FloatFloatEvent();
    public StringEvent OnProgressChangeText = new StringEvent();


    ////// SINGLE INSTANCE ////////////
    public static Pool Instance;
    ///////////////////////////////////

    private void Awake()
    {
        Instance = this;
    }

    public void StartPool()
    {
        // Get total number of items to spawn (this is only for the loading bar to know total progress)
        totalSpawned = 0;
        totalProgress = 0;

        for (int i = 0; i < itemsToPool.Count; ++i)
        {
            if (itemsToPool[i].doSpawn == false) continue;

            if ((int)(itemsToPool[i].amount * scale) > maxSize) maxSize = (int)(itemsToPool[i].amount * scale);            

            totalProgress += (int)(itemsToPool[i].amount * scale);
        }

        pooledObjects = new GameObject[itemsToPool.Count, maxSize];

        StartCoroutine(EAwake());
    }

    // Start is called before the first frame update
    IEnumerator EAwake()
    {
        OnPoolCreateStart?.Invoke();

        yield return null;

        int size = itemsToPool.Count;
        for (int x = size - 1; x >= 0; --x) //for each pooleable object
        {
            if (itemsToPool[x].doSpawn == false) continue;

            GameObject go = new GameObject(); go.name = "[Object Pool - " + itemsToPool[x].objectToPool.name + "]";
            go.transform.position = Vector3.zero; go.transform.rotation = Quaternion.identity;
            if (folder != null) go.transform.parent = folder;

            itemsToPool[x].parentObj = go;

            // Create a reference of the prefab object from which to clone the thousands from
            // so we can add temporary scripts to it, and the pooled ones will auto-have them
            GameObject clonedPrefab = Instantiate(itemsToPool[x].objectToPool);
            UtilityFunctions.UpdateLODlevels(clonedPrefab.transform);
            clonedPrefab.SetActive(false);

            int amountSpawn = (int)(itemsToPool[x].amount * scale);
            for (int i = 0; i < amountSpawn; i++)
            {
                GameObject obj = Instantiate(clonedPrefab);                

                obj.transform.parent = itemsToPool[x].parentObj.transform; pooledObjects[x, i] = obj;

                // Progress bar stuff (purely)
                totalSpawned += 1;
                delaySetCounter += 1;
                if (delaySetCounter >= delayAfterEverySet || totalSpawned == 1)
                {
                    yield return null;
                    delaySetCounter = 0;
                    OnProgressChange?.Invoke(totalSpawned, totalProgress);
                    OnProgressChangeText?.Invoke("Pooling asset: " + itemsToPool[x].objectToPool.name);
                    yield return null;

                }
                ///////////////////////////////
            }
            DestroyImmediate(clonedPrefab);
        }

        OnProgressChange?.Invoke(totalSpawned, totalProgress);
        OnProgressChangeText?.Invoke("Loading complete");

        yield return new WaitForSeconds(0.5f);

        OnPoolCreateEnd?.Invoke();
    }

    //  OUTDATED - used to use string comparison
    //public GameObject Instantiate_OLD(string nom, Vector3 position, Quaternion rotation, bool usePoolsFolder = true, bool ignoreActive = false)
    //{
    //    // 1.0  First, search for unused item in pool to re-use it
    //    for (int i = 0; i < pooledObjects.Count; i++)
    //    {
    //        if (pooledObjects[i] != null &&
    //            (ignoreActive || (!pooledObjects[i].activeInHierarchy && !pooledObjects[i].activeSelf)) &&
    //            pooledObjects[i].name.Contains(nom))
    //        {
    //            pooledObjects[i].gameObject.transform.position = position;
    //            pooledObjects[i].gameObject.transform.rotation = rotation;
    //            pooledObjects[i].SetActive(true);

    //            return pooledObjects[i];
    //        }
    //    }

    //    // 2.0 Being here means we were out of amount, so we search for that PoolItem through list and expand it
    //    return MoreNeeded(nom, usePoolsFolder);
    //}

    public GameObject Instantiate(int id, Vector3 position, Quaternion rotation, bool usePoolsFolder = true, bool ignoreActive = false)
    {
        // 1.0  First, search for unused item in pool to re-use it
        for (int i = 0; i < maxSize; i++)
        {
            if (pooledObjects[id, i] != null &&
                (ignoreActive || (!pooledObjects[id, i].activeInHierarchy && !pooledObjects[id, i].activeSelf)) )
            {
                pooledObjects[id, i].gameObject.transform.position = position;
                pooledObjects[id, i].gameObject.transform.rotation = rotation;
                pooledObjects[id, i].SetActive(true);

                return pooledObjects[id, i];
            }
        }

        return null; // MoreNeeded(nom, usePoolsFolder);
    }

    //private GameObject MoreNeeded(string nom, bool usePoolFolder = true)
    //{
    //    Debug.Log("System: More amount of " + nom + " was needed than pooler currently holds.");
    //    foreach (PoolItem item in itemsToPool)
    //    {
    //        if (item.objectToPool.name.Contains(nom) && item.canExpand)
    //        {
    //            GameObject obj = (GameObject)Instantiate(item.objectToPool);
    //            obj.transform.position = Vector3.zero;
    //            obj.transform.rotation = Quaternion.identity;
    //            if (usePoolFolder) obj.transform.SetParent(item.parentObj.transform);
    //            obj.SetActive(true); pooledObjects.Add(obj); return obj;
    //        }
    //    }
    //    return null;
    //}


    // END OF FILE //////////////
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public Transform folder;
    public Transform waterVolume;
    // list that describes each object pooled object
    public List<PoolItem> itemsToPool;

    // list that holds all pooledobject lists
    public List<GameObject> pooledObjects;

    [Space(20)]
    public UnityEvent OnPoolCreateStart = new UnityEvent();
    public UnityEvent OnPoolCreateEnd = new UnityEvent();

    ////// SINGLE INSTANCE ////////////
    public static Pool Instance;
    //void Awake() { instance = this; }
    ///////////////////////////////////

    private void Awake()
    {
        Instance = this;        
    }

    private void Start()
    {
        StartCoroutine(EAwake());
    }

    // Start is called before the first frame update
    IEnumerator EAwake()
    {
        OnPoolCreateStart?.Invoke();
        yield return null;

        pooledObjects = new List<GameObject>(); //list that holds all pooleable objects

        int size = itemsToPool.Count;
        for (int x = size - 1; x >= 0; --x) //for each pooleable object
        {
            if (itemsToPool[x] == null) continue;
            if (itemsToPool[x].objectToPool == null) continue;
            if (itemsToPool[x].doSpawn == false) continue;
            GameObject go = new GameObject(); go.name = "[Object Pool - " + itemsToPool[x].objectToPool.name + "]";
            go.transform.position = Vector3.zero; go.transform.rotation = Quaternion.identity;
            if (folder != null) go.transform.parent = folder;
            for (int i = 0; i < itemsToPool[x].amount; i++)
            {
                GameObject obj = (GameObject)Instantiate(itemsToPool[x].objectToPool);
                if (!itemsToPool[x].customFolder) itemsToPool[x].parentObj = go;
                else itemsToPool[x].parentObj = itemsToPool[x].customFolder;
                obj.transform.position = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;
                obj.transform.parent = itemsToPool[x].parentObj.transform; obj.SetActive(false); pooledObjects.Add(obj);
            }
            yield return null;
        }

        OnPoolCreateEnd?.Invoke();
    }

    //   Retrieving an object from the pool by its name
    public GameObject Instantiate(string nom, Vector3 position, Quaternion rotation, bool usePoolsFolder = true, bool ignoreActive = false)
    {
        // 1.0  First, search for unused item in pool to re-use it
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i] != null   &&
                (  ignoreActive   ||   (!pooledObjects[i].activeInHierarchy && !pooledObjects[i].activeSelf)  )   &&
                pooledObjects[i].name.Contains(nom))
            {
                pooledObjects[i].gameObject.transform.position = position;
                pooledObjects[i].gameObject.transform.rotation = rotation;
                pooledObjects[i].SetActive(true);
                totalSpawned += 1;

                return pooledObjects[i];
            }
        }

        // 2.0 Being here means we were out of amount, so we search for that PoolItem through list and expand it
        return MoreNeeded(nom, usePoolsFolder);
    }


    // Polymorphism
    //public GameObject Instantiate(string nom, Transform parent)
    //{
    //    // 1.0  First, search for unused item in pool to re-use it
    //    for (int i = 0; i < pooledObjects.Count; i++)
    //    {
    //        if (pooledObjects[i] != null && (!pooledObjects[i].activeInHierarchy && !pooledObjects[i].activeSelf) && pooledObjects[i].name.Contains(nom))
    //        {
    //            pooledObjects[i].gameObject.transform.position = Vector3.zero;
    //            pooledObjects[i].gameObject.transform.rotation = Quaternion.identity;
    //            pooledObjects[i].gameObject.transform.SetParent(parent);

    //            pooledObjects[i].SetActive(true);
    //            return pooledObjects[i];
    //        }
    //    }
    //    // 2.0 Being here means we were out of amount, so we search for that PoolItem through list and expand it
    //    return MoreNeeded(nom);
    //}

    private GameObject MoreNeeded(string nom, bool usePoolFolder = true)
    {
        Debug.Log("System: More amount of " + nom + " was needed than pooler currently holds.");
        foreach (PoolItem item in itemsToPool)
        {
            if (item.objectToPool.name.Contains(nom) && item.canExpand)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.transform.position = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;
                if (usePoolFolder) obj.transform.SetParent(item.parentObj.transform);
                obj.SetActive(true); pooledObjects.Add(obj); return obj;
            }
        }
        return null;
    }


    // END OF FILE //////////////
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolItem  // a single instance of "List<ObjectPoolItem> itemsToPool" means all this info
{
    public GameObject objectToPool;
    public GameObject parentObj;
    public int amount;
    public int currentlyActive = 0;
    public bool canExpand;
    public string notes;
}

public class Pool : MonoBehaviour
{
    public int spawned = 0;
    public Transform folder;
    // list that describes each object pooled object
    public List<PoolItem> itemsToPool;

    // list that holds all pooledobject lists
    public List<GameObject> pooledObjects;

    ////// SINGLE INSTANCE ////////////
    public static Pool Instance;
    //void Awake() { instance = this; }
    ///////////////////////////////////

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        pooledObjects = new List<GameObject>(); //list that holds all pooleable objects

        foreach (PoolItem item in itemsToPool) //for each pooleable object
        {
            if (item == null) continue;
            if (item.objectToPool == null) continue;
            GameObject go = new GameObject(); go.name = "[Object Pool - " + item.objectToPool.name + "]";
            go.transform.position = Vector3.zero; go.transform.rotation = Quaternion.identity;
            if (folder != null) go.transform.parent = folder;
            for (int i = 0; i < item.amount; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool); item.parentObj = go;
                obj.transform.position = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;
                obj.transform.parent = item.parentObj.transform; obj.SetActive(false); pooledObjects.Add(obj);
            }
        }
    }

    //   Retrieving an object from the pool by its name
    public GameObject Instantiate(string nom, Vector3 position, Quaternion rotation, bool usePoolsFolder = true)
    {
        // 1.0  First, search for unused item in pool to re-use it
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i] != null && (!pooledObjects[i].activeInHierarchy && !pooledObjects[i].activeSelf) && pooledObjects[i].name.Contains(nom))
            {
                pooledObjects[i].gameObject.transform.position = position;
                pooledObjects[i].gameObject.transform.rotation = rotation;
                pooledObjects[i].SetActive(true);

                spawned++;
                return pooledObjects[i];
            }
        }
        // 2.0 Being here means we were out of amount, so we search for that PoolItem through list and expand it
        return MoreNeeded(nom, usePoolsFolder);        
    }

    
    // Polymorphism
    public GameObject Instantiate(string nom, Transform parent)
    {
        // 1.0  First, search for unused item in pool to re-use it
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i] != null && (!pooledObjects[i].activeInHierarchy && !pooledObjects[i].activeSelf) && pooledObjects[i].name.Contains(nom))
            {
                pooledObjects[i].gameObject.transform.position = Vector3.zero;
                pooledObjects[i].gameObject.transform.rotation = Quaternion.identity;
                pooledObjects[i].gameObject.transform.SetParent(parent);

                pooledObjects[i].SetActive(true);
                return pooledObjects[i];
            }
        }
        // 2.0 Being here means we were out of amount, so we search for that PoolItem through list and expand it
        return MoreNeeded(nom);
    }

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

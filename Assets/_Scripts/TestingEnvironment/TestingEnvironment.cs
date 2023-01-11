using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestingEnvironment : MonoBehaviour
{
    [Header("Properties:")]
    public int radius = 5;
    public GameObject[] blockPrefabs;
    public GameObject player;
    public Transform blockFolder;
    public GameObject[] enemyPrefabs;
    public int numOfEnemies = 4;

    // Run time variables
    private List<List<GameObject>> list;
    private NavMeshSurface _surface;


    // Start is called before the first frame update
    public void DeveloperMap()
    {
        list = new List<List<GameObject>>();

        for (int x = -radius; x < radius; ++x)
        {
            List<GameObject> xList = new List<GameObject>();
            for (int z = -radius; z < radius; ++z)
            {
                GameObject obj = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)], new Vector3(x, 0, z), Quaternion.identity);
                xList.Add(obj);
                if (xList.Count == 1) _surface = obj.AddComponent<NavMeshSurface>();
                obj.transform.SetParent(blockFolder);
            }
            list.Add(xList);
        }

        for (int x = -radius; x < 0; ++x)
        {
            List<GameObject> xList = new List<GameObject>();
            for (int z = -radius; z < radius; ++z)
            {
                GameObject obj = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Length)], new Vector3(x, 0.49f, z), Quaternion.identity);
                xList.Add(obj);
                obj.transform.SetParent(blockFolder);
            }
            list.Add(xList);
        }

        if (numOfEnemies > 0)
        {
            NavMeshBuilder.Instance.surface = _surface;
            NavMeshBuilder.Instance.BuildNavMesh();

            for (int i = 0; i < numOfEnemies; ++i)
            {
                float xOffset = Random.Range(-0.5f, 0.5f);
                float zOffset = Random.Range(-0.5f, 0.5f);

                GameObject obj = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], list[6][6].transform.position + new Vector3(xOffset, 1.5f, zOffset), Quaternion.identity);
            }
        }

        Vector3 spawnSpot = list[radius - 2][radius + 2].transform.position;
        player.transform.position = spawnSpot + new Vector3(0, 2, 0);
    }


}

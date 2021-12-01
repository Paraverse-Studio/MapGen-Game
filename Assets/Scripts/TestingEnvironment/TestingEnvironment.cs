using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingEnvironment : MonoBehaviour
{
    public int radius = 5;
    public GameObject blockPrefab;
    public MobController player;
    public Transform blockFolder;
    private List<List<GameObject>> list;
    public GameObject enemyPrefab;
    public GameObject enemy2Prefab;
    public int numOfEnemies = 4;

    // Start is called before the first frame update
    public void DeveloperMap()
    {
        list = new List<List<GameObject>>();

        for (int x = -radius; x < radius; ++x)
        {
            List<GameObject> xList = new List<GameObject>();
            for (int z = -radius; z < radius; ++z)
            {
                GameObject obj = Instantiate(blockPrefab, new Vector3(-20 - x, 0, -20 - z), Quaternion.identity);
                xList.Add(obj);
                obj.transform.SetParent(blockFolder);
            }
            list.Add(xList);
        }

        for (int x = -radius; x < 0; ++x)
        {
            List<GameObject> xList = new List<GameObject>();
            for (int z = -radius; z < radius; ++z)
            {
                GameObject obj = Instantiate(blockPrefab, new Vector3(-20 - x, 1, -20 - z), Quaternion.identity);
                xList.Add(obj);
                obj.transform.SetParent(blockFolder);
            }
            list.Add(xList);
        }

        for (int i = 0; i < numOfEnemies/2; ++i)
        {
            float xOffset = Random.Range(-1.5f, 1.5f);
            float zOffset = Random.Range(-1.5f, 1.5f);

            GameObject obj = Instantiate(enemyPrefab, list[6][6].transform.position + new Vector3(xOffset,1.5f,zOffset), Quaternion.identity);
        }
        for (int i = 0; i < numOfEnemies/2; ++i)
        {
            float xOffset = Random.Range(-1.5f, 1.5f);
            float zOffset = Random.Range(-1.5f, 1.5f);

            GameObject obj = Instantiate(enemy2Prefab, list[6][6].transform.position + new Vector3(xOffset, 1.5f, zOffset), Quaternion.identity);
        }

        Vector3 spawnSpot = list[radius-2][radius+2].transform.position;
        player.TeleportPlayer(spawnSpot + new Vector3(0, 2, 0));
    }


}

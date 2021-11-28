using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingEnvironment : MonoBehaviour
{
    public int radius = 5;
    public GameObject blockPrefab;
    public PlayerController player;
    public Transform blockFolder;
    private List<List<GameObject>> list;

    // Start is called before the first frame update
    void Start()
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

        Vector3 spawnSpot = list[radius-2][radius+2].transform.position;
        player.TeleportPlayer(spawnSpot + new Vector3(0, 2, 0));
    }


}

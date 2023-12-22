using Paraverse.Mob.Stats;
using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR

using UnityEngine;

public class Debugging : MonoBehaviour
{
    public GameObject chest;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Heal All
        if (Input.GetKeyDown(KeyCode.Y))
        {
            MobStats[] stats = FindObjectsOfType<MobStats>();

            foreach (MobStats s in stats)
            {
                s.GetComponentInChildren<MobStats>().SetFullHealth();
            } 
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (GlobalSettings.Instance.testGameObject)
            {
                //Vector3 r = Vector3.down * (Random.value < 0.5f ? 90f : 180f);
                GameObject g = Instantiate(GlobalSettings.Instance.testGameObject,
                    GlobalSettings.Instance.player.transform.position,
                    GlobalSettings.Instance.player.transform.rotation);
                //g.GetComponent<ChestObject>().Initialize(Random.Range(0,3));

                ChestObject c = Instantiate(chest, GlobalSettings.Instance.player.transform.position + transform.forward * 2,
                    GlobalSettings.Instance.player.transform.rotation).GetComponent<ChestObject>();
                c.Initialize(ChestObject.ChestTierType.Common);
            }
        }
    }


}

#endif

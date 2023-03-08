using Paraverse.Mob.Stats;
using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR

using UnityEngine;

public class Debugging : MonoBehaviour
{

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
                Instantiate(GlobalSettings.Instance.testGameObject, 
                    GlobalSettings.Instance.player.transform.position + new Vector3(0, 0.5f, 0) + (GlobalSettings.Instance.player.transform.forward * 3f), 
                    GlobalSettings.Instance.player.transform.rotation);
        }
    }


}

#endif

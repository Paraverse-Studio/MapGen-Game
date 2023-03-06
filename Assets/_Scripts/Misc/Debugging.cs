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
    }


}

#endif

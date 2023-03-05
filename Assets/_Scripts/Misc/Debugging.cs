using Paraverse.Mob.Stats;
using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR

using UnityEngine;

public class Debugging : MonoBehaviour
{
    StatModifier buff;
    bool on = false;
    MobStats stats;

    private void Start()
    {
        stats = GlobalSettings.Instance.player.GetComponentInChildren<MobStats>();
        buff = new StatModifier(0);
        stats.MoveSpeed.AddMod(buff);
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

            on = !on;
            buff.Value = on? -2f : 0f;
            

        }
    }


}

#endif

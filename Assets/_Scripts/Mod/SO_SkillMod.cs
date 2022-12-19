using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/SO_SkillMod")]
public class SO_SkillMod : SO_Mod
{
    [Header("Obtained Items")]
    public GameObject[] obtainedSkills;

    public override void Activate()
    {
        base.Activate();
        Debug.Log("ACTIVATED SKILL SHIT");
    }
}

using Paraverse.Combat;
using Paraverse.Mob.Combat;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle Mob skills here, as basic attacks are handled in the inherited MobCombat.cs. 
/// If any code in MobCombat.cs needs to be alter, just override the method within this script. 
/// </summary>
public class EnhancedMobCombat : MobCombat
{
    [SerializeField, Tooltip("Mob skills.")]
    protected List<MobSkill> skills = new List<MobSkill>();

    protected override void Update()
    {
        base.Update();

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].SkillUpdate();
        }
    }
}

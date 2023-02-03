using Paraverse;
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
    #region Animation Events Skill One
    public override void FireProjectile()
    {
        MobSkill skill;

        if (IsSkilling)
        {
            skill = skills[usingSkillIdx];
        }
        else
            skill = basicAttackSkill;

        // Archers may hold an arrow which needs to be set to off/on when firing
        if (skill.projData.projHeld != null)
            skill.projData.projHeld.SetActive(false);

        Vector3 playerPos = new Vector3(player.position.x, player.position.y + 0.5f, player.position.z);
        Vector3 targetDir = (playerPos - transform.position).normalized;

        Quaternion lookRot;
        if (skill.projData.projRotation == null)
            lookRot = Quaternion.LookRotation(targetDir);
        else
            lookRot = skill.projData.projRotation.rotation;

        // Instantiate and initialize projectile
        GameObject go = Instantiate(skill.projData.projPf, skill.projData.projOrigin.position, lookRot);
        Projectile proj = go.GetComponent<Projectile>();
        proj.Init(this, targetDir, skill.projData.basicAtkProjSpeed, skill.scalingStatData);
    }
    #endregion
}

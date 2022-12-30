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
    private int usingSkillIdx;
    [SerializeField, Tooltip("Mob skills.")]
    protected List<MobSkill> skills = new List<MobSkill>();
    [SerializeField]
    protected string animBool = "isUsingSkill";
    public bool IsSkilling { get { return _isSkilling; } }
    protected bool _isSkilling = false;

    protected override void Update()
    {
        base.Update();
        bool usingSkill = false;
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].SkillUpdate();
            if (skills[i].skillOn)
            {
                usingSkill = true;
                usingSkillIdx = i;
            }
        }
         anim.SetBool(StringData.IsUsingSkill, usingSkill);
        _isSkilling = anim.GetBool(StringData.IsSkilling);
    }

    public override void FireProjectile()
    {
        ProjectileData data;

        if (IsSkilling)
            data = skills[usingSkillIdx].projData;
        else
            data = projData;
        
        // Archers may hold an arrow which needs to be set to off/on when firing
        if (data.projHeld != null)
            data.projHeld.SetActive(false);

        Vector3 playerPos = new Vector3(player.position.x, player.position.y + 0.5f, player.position.z);
        Vector3 targetDir = (playerPos - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(targetDir);

        // Instantiate and initialize projectile
        GameObject go = Instantiate(projData.projPf, projData.projOrigin.position, lookRot);
        Projectile proj = go.GetComponent<Projectile>();
        proj.Init(this, targetDir, projData.basicAtkProjSpeed, basicAtkRange, basicAtkDmgRatio * stats.AttackDamage.FinalValue);
    }
}

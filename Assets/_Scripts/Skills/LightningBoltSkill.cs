using Paraverse.Combat;
using Paraverse.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltSkill : MobSkill, IMobSkill
{
    #region variables
    [Header("Lightning Bolt Properties")]
    [SerializeField]
    private int rangeLimit;
    [SerializeField]
    private GameObject FX;

    private bool performing = false;
    #endregion


    // Check if target exists
    protected override bool CanUseSkill()
    {
        if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && IsBasicAttack == false
            && null != mob.Target && UtilityFunctions.IsDistanceLessThan(mob.transform.position, mob.Target.position, rangeLimit))
            return true;

        return false;
    }

    protected override void ExecuteSkillLogic()
    {
        performing = true;
        base.ExecuteSkillLogic();

        // teleport
        if (null == mob.Target) return; 

        anim.SetBool(StringData.IsInteracting, true);

        if (FX)
        {
            Instantiate(FX, mob.Target.transform.position, Quaternion.identity);
        }

        DisableSkill();

        StartCoroutine(UtilityFunctions.IDelayedAction(0.2f, () =>
        {
            performing = false;
            anim.SetBool(StringData.IsInteracting, false);
            anim.SetBool(StringData.IsUsingSkill, false);
        }));
    }

}

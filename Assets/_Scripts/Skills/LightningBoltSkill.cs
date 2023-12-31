using Paraverse;
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
        if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && IsBasicAttack == false)
        {
            if (null == mob.Target)
            {
                mob.Target = SelectableSystem.Instance.ToggleSelect();
            }

            if (null != mob.Target && UtilityFunctions.IsDistanceLessThan(mob.transform.position, mob.Target.position, rangeLimit))
            {
                return true;
            }
        }

        return false;
    }

    protected override void ExecuteSkillLogic()
    {
        performing = true;
        base.ExecuteSkillLogic();

        // need a target to cast this
        if (null == mob.Target) return; 

        anim.SetBool(StringData.IsInteracting, true);

        if (FX)
        {
            GameObject go = Instantiate(FX, mob.Target.transform.position, Quaternion.identity);
            Projectile proj = go.GetComponent<Projectile>();
            proj.Init(mob, mob.Target.transform.position, scalingStatData);
        }

        OnSkillExecuted();

        StartCoroutine(UtilityFunctions.IDelayedAction(0.2f, () =>
        {
            performing = false;
            anim.SetBool(StringData.IsInteracting, false);
            anim.SetBool(StringData.IsUsingSkill, false);
        }));
    }

}

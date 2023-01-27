using Paraverse.Combat;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RootSkill : MobSkill, IMobSkill
{
    #region Variables
    [SerializeField]
    [Header("Root Skill")]
    private float rootDuration = 3f;
    private float curRootDuration;
    #endregion


    #region Inherited Methods
    public override void SkillUpdate()
    {
        base.SkillUpdate();
        RootHandler();
    }

    protected override void ExecuteSkillLogic()
    {
        mob.IsSkilling = true;
        skillOn = true;
        anim.SetBool(StringData.IsUsingSkill, true);
        curRootDuration = rootDuration;
        stats.UpdateCurrentEnergy(-cost);
        anim.Play(animName);
    }

    protected override bool CanUseSkill()
    {
        if (IsOffCooldown && HasEnergy && TargetWithinRange)
        {
            return true;
        }

        // Debug.Log(_skillName + " is on cooldown or don't have enough energy!");
        return false;
    }
    #endregion

    #region Private Methods
    private void RootHandler()
    {
        if (skillOn == false) return;

        if (curRootDuration <= 0)
        {
            DisableSkill();
            curCooldown = cooldown;
        }
        else
        {
            curRootDuration -= Time.deltaTime;
        }
    }
    #endregion
}

using Paraverse.Combat;
using UnityEngine;

public class RootSkill : MobSkill, IMobSkill
{

    #region Variables
    [SerializeField]
    private float rootDuration = 3f;
    private float curRootDuration;
    #endregion


    #region Public Methods
    public override void SkillUpdate()
    {
        base.SkillUpdate();
        RootHandler();
    }
    public override void Execute()
    {
        if (CanUseSkill())
        {
            mob.IsSkilling = true;
            skillOn = true;
            curRootDuration = rootDuration;
            stats.UpdateCurrentEnergy(-cost);
            anim.Play(animName);
            Debug.Log("Executing skill: " + _skillName + " which takes " + cost + " points of energy out of " + stats.CurEnergy + " point of current energy." +
                "The max cooldown for this skill is " + cooldown + " and the animation name is " + animName + ".");

            // depending on the skill, 
            // if it's a projectile, set its damage to:
            // int damage = (flatPower) + (mobStats.AttackDamage.FinalValue * attackScaling) + (mobStats.AbilityPower.FinalValue * abilityScaling);
        }
    }
    #endregion

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

    /// <summary>
    /// Returns true if skill conditions are met. 
    /// </summary>
    /// <returns></returns>
    protected override bool CanUseSkill()
    {
        if (IsOffCooldown && HasEnergy && TargetWithinRange)
        {
            return true;
        }

        Debug.Log(_skillName + " is on cooldown or don't have enough energy!");
        return false;
    }
}

using Paraverse;
using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;

public class KickSkill : MobSkill, IMobSkill
{

    #region Variables
    [SerializeField]
    protected GameObject kickAttackColliderGO;
    protected AttackCollider kickAttackCollider;
    #endregion

    #region Public Methods
    public override void ActivateSkill(MobCombat mob, Animator anim, IMobStats stats, Transform target = null)
    {
        this.mob = mob;
        this.target = target;
        this.anim = anim;
        this.stats = stats;
        curCooldown = 0f;
        if (mob.tag.Equals(StringData.PlayerTag))
            input.OnSkillOneEvent += Execute;

        if (null == kickAttackColliderGO && null != kickAttackCollider)
        {
            kickAttackCollider = kickAttackColliderGO.GetComponent<AttackCollider>();
            kickAttackCollider.Init(mob, stats);
        }
        kickAttackColliderGO.SetActive(true);
        kickAttackCollider = kickAttackColliderGO.GetComponent<AttackCollider>();
        kickAttackCollider.Init(mob, stats);
        kickAttackColliderGO.SetActive(false);
    }

    /// <summary>
    /// Contains all methods required to run in Update within MobCombat script.
    /// </summary>
    public override void SkillUpdate()
    {
        if (null != target && mob.IsBasicAttacking == false && anim.GetBool(StringData.IsUsingSkill) == false)
        {
            Execute();
        }

        if (anim.GetBool(StringData.IsUsingSkill) == false)
            skillOn = false;
        CooldownHandler();
    }

    /// <summary>
    /// Responsible for executing skill on button press.
    /// </summary>
    public override void Execute()
    {
        if (CanUseSkill() && skillOn == false)
        {
            anim.SetBool(StringData.IsUsingSkill, true);
            skillOn = true;
            curCooldown = cooldown;
            stats.UpdateCurrentEnergy(-cost);
            anim.Play(animName);
            Debug.Log("Executing skill: " + _skillName + " which takes " + cost + " points of energy out of " + stats.CurEnergy + " point of current energy." +
                "The max cooldown for this skill is " + cooldown + " and the animation name is " + animName + ".");

            // depending on the skill, 
            // if it's a projectile, set its damage to:
            // int damage = (flatPower) + (mobStats.AttackDamage.FinalValue * attackScaling) + (mobStats.AbilityPower.FinalValue * abilityScaling);
        }
    }

    public void EnableKickAttackCollider()
    {
        if (kickAttackColliderGO != null)
            kickAttackColliderGO.SetActive(true);
    }

    public void DisableKickAttackCollider()
    {

        if (kickAttackColliderGO != null)
            kickAttackColliderGO.SetActive(false);
    }
    #endregion
}

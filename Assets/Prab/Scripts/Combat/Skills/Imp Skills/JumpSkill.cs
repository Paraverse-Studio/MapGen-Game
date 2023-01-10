using Paraverse;
using Paraverse.Combat;
using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class JumpSkill : MobSkill, IMobSkill
{
    #region Variables
    protected MobController mobController;
    #endregion

    #region Public Methods
    public override void ActivateSkill(MobCombat mob, Animator anim, IMobStats stats, Transform target = null)
    {
        this.mob = mob;
        mobController = mob.GetComponent<MobController>();
        mobController.OnLandEvent += DisableSkill;
        this.target = target;
        this.anim = anim;
        this.stats = stats;
        curCooldown = 0f;
        if (mob.tag.Equals(StringData.PlayerTag))
            input.OnSkillOneEvent += Execute;

        if (null == attackCollider && null != attackColliderGO)
        {
            attackCollider = attackColliderGO.GetComponent<AttackCollider>();
            attackCollider.Init(mob, stats);
        }
    }

    public override void DeactivateSkill(PlayerInputControls input)
    {
        base.DeactivateSkill(input);
        mobController.OnLandEvent -= DisableSkill;
    }

    /// <summary>
    /// Responsible for executing skill on button press.
    /// </summary>
    public override void Execute()
    {
        if (CanUseSkill())
        {
            mob.IsSkilling = true;
            skillOn = true;
            anim.SetBool(StringData.IsUsingSkill, true);
            curCooldown = cooldown;
            stats.UpdateCurrentEnergy(-cost);
            anim.Play(animName);
            mobController.ApplyJump(target.transform.position);
            Debug.Log("Executing skill: " + _skillName + " which takes " + cost + " points of energy out of " + stats.CurEnergy + " point of current energy." +
                "The max cooldown for this skill is " + cooldown + " and the animation name is " + animName + ".");

            // depending on the skill, 
            // if it's a projectile, set its damage to:
            // int damage = (flatPower) + (mobStats.AttackDamage.FinalValue * attackScaling) + (mobStats.AbilityPower.FinalValue * abilityScaling);
        }
    }
    #endregion

    #region Private Methods

    private void DisableSkill()
    {
        skillOn = false;
        anim.SetBool(StringData.IsGrounded, true);
    }
    #endregion
}

using Paraverse.Combat;
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


    #region Inherited Methods
    public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, anim, stats, target);
        if (mobController == null) mobController = mob.GetComponent<MobController>();
        mobController.OnLandEvent += DisableSkill;
    }

    public override void DeactivateSkill(PlayerInputControls input)
    {
        base.DeactivateSkill(input);
        mobController.OnLandEvent -= DisableSkill;
    }

    protected override void ExecuteSkillLogic()
    {
        mob.IsUsingSkilling = true;
        skillOn = true;
        anim.SetBool(StringData.IsUsingSkill, true);
        curCooldown = cooldown;
        stats.UpdateCurrentEnergy(-cost);
        anim.Play(animName);
        mobController.ApplyJump(target.transform.position);
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();
        anim.SetBool(StringData.IsGrounded, true);
    }
    #endregion
}

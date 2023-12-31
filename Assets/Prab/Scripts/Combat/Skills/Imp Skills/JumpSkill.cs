using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
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
    mobController.OnLandEvent += OnSkillComplete;
  }

  protected override void ExecuteSkillLogic()
  {
    mob.IsSkilling = true;
    skillOn = true;
    anim.SetBool(StringData.IsUsingSkill, true);
    _curCooldown = _cooldown;
    stats.UpdateCurrentEnergy(-cost);
    anim.Play(animName);
    mobController.ApplyJump(target.transform.position);
  }

  protected override void OnSkillComplete()
  {
    base.OnSkillComplete();
    anim.SetBool(StringData.IsGrounded, true);
  }
  #endregion
}

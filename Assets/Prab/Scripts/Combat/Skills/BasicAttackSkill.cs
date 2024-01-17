using Paraverse.Combat;
using UnityEngine;

public class BasicAttackSkill : MobSkill, IMobSkill
{
  protected virtual void Start()
  {
    _isBasicAttack = true;
  }
  
  public override void SkillUpdate()
  {
    if (null != target && mob.IsAttacking == false && Input == null)
      Execute();

    TargetLockDuringSkill();
    SkillStateManager();
    CooldownHandler();
  }

  public void ExecuteBasicAttack()
  {
    Execute();
  }

  protected override void ExecuteSkillLogic()
  {
    mob.IsBasicAttacking = true;
    SetSkillState(SkillState.InUse);
    anim.SetBool(StringData.IsBasicAttacking, true);
    _curCooldown = _cooldown;
    anim.Play(animName);
    curSkillStateToCompleteTimer = skillStateToCompleteTimer;
  }

  protected override bool CanUseSkill()
  {
    if (IsOffCooldown && TargetWithinRange && mob.IsAttackLunging == false && mob.IsAttacking == false && mob.ActiveSkill == null || 
      IsOffCooldown && TargetWithinRange && mob.IsAttackLunging == false && mob.IsAttacking == false && input != null)
      return true;

    return false;
  }
}

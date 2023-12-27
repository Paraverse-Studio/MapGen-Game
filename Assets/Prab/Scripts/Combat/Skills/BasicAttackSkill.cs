using Paraverse.Mob.Combat;

public class BasicAttackSkill : MobSkill, IMobSkill
{
  public delegate void OnExecuteBasicAttackDel();
  public event OnExecuteBasicAttackDel OnExecuteBasicAttackEvent;

  //public override void SkillUpdate()
  //{
  //  RotateToTarget();
  //  CooldownHandler();
  //}

  public void ExecuteBasicAttack()
  {
    Execute();
  }

  protected override void ExecuteSkillLogic()
  {
    curCooldown = cooldown;
    anim.Play(animName);
    anim.SetBool(StringData.IsBasicAttacking, true);
  }

  protected override bool CanUseSkill()
  {
    if (IsOffCooldown && TargetWithinRange && mob.IsAttackLunging == false && mob.IsUsingSkilling == false)
      return true;

    return false;
  }

  /// <summary>
  /// Responsible for executing skill on button press.
  /// </summary>
  protected override void Execute()
  {
    if (CanUseSkill())
    {
      OnExecuteBasicAttackEvent?.Invoke();
      SubscribeAnimationEventListeners();
      ExecuteSkillLogic();
    }
  }
}

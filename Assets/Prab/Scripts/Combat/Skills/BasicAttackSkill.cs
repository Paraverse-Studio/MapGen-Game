using Paraverse.Combat;

public class BasicAttackSkill : MobSkill, IMobSkill
{
    public override void SkillUpdate()
    {
        TargetLockDuringSkill();
        CooldownHandler();
    }

    public void ExecuteBasicAttack()
    {
        Execute();
    }

    protected override void ExecuteSkillLogic()
    {
        _curCooldown = _cooldown;
        anim.Play(animName);
        anim.SetBool(StringData.IsBasicAttacking, true);
    }

    protected override bool CanUseSkill()
    {
        if (IsOffCooldown && TargetWithinRange && mob.IsAttackLunging == false && mob.IsSkilling == false)
            return true;

        return false;
    }
}

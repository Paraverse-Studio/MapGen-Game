using Paraverse.Combat;

public class BasicAttackSkill : MobSkill, IMobSkill
{
    public override void SkillUpdate()
    {
        RotateToTarget();
        CooldownHandler();
    }

    public void ExecuteBasicAttack()
    {
        Execute();
    }

    protected override void ExecuteSkillLogic()
    {
        curCooldown = cooldown;
        anim.Play(animName);
    }

    protected override bool CanUseSkill()
    {
        if (IsOffCooldown && TargetWithinRange && mob.IsAttackLunging == false)
            return true;

        return false;
    }
}

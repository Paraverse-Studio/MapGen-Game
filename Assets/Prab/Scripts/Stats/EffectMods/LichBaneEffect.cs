using Paraverse;
using Paraverse.Mob.Stats;
using Paraverse.Stats;
using UnityEngine;

//After using a skill, the next basic attack deals 100% extra Attack damage (40/80/120)
public class LichBaneEffect : MobEffect
{
    private StatModifier _mod;

    private bool _effectApplied = false;

    public override void ActivateEffect(MobStats stats)
    {
        base.ActivateEffect(stats);
        _mod = new StatModifier(scalingStatData.FinalValue(_stats));
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackPostHitEvent += RemoveMod;
        OnSkillChangeApplyEffect();
    }

    public override void DeactivateEffect()
    {
        base.DeactivateEffect();
        RemoveMod();
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackPostHitEvent += RemoveMod;
        if (null != _combat.ActiveSkill)
            _combat.ActiveSkill.OnExecuteSkillEvent -= ApplyEffect;
    }

    public override void AddSubscribersToSkillEvents(Damage col)
    {
        base.AddSubscribersToSkillEvents(col);
    }

    public override void RemoveSubscribersToSkillEvents(Damage col)
    {
        base.RemoveSubscribersToSkillEvents(col);
    }

    public override void OnSkillChangeApplyEffect()
    {
        base.OnSkillChangeApplyEffect();
        if (null != _combat.ActiveSkill)
            _combat.ActiveSkill.OnExecuteSkillEvent += ApplyEffect;
    }

    private void ApplyEffect()
    {
        if (_effectApplied) return;

        _effectApplied = true;
        _mod.Value = scalingStatData.FinalValue(_stats);
        _stats.AttackDamage.AddMod(_mod);
    }


    private void RemoveMod()
    {
        _effectApplied = false;
        _stats.AttackDamage.RemoveMod(_mod);
    }
}

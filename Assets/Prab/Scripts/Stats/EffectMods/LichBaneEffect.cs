using Paraverse;
using Paraverse.Mob.Stats;
using Paraverse.Stats;
using UnityEngine;

//After using a skill, the next basic attack deals 100% extra Attack damage (40/80/120)
public class LichBaneEffect : MobEffect
{
    private StatModifier _mod;

    private bool _applyEffect = false;

    public override void ActivateEffect(MobStats stats)
    {
        base.ActivateEffect(stats);
        _mod = new StatModifier(scalingStatData.FinalValue(_stats));
        //_combat.BasicAttackSkill.attackCollider.OnBasicAttackPreHitEvent += ApplyEffect;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent += RemoveMod;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent += DisableApplyEffect;
    }

    public override void DeactivateEffect()
    {
        base.DeactivateEffect();
        RemoveMod();
        //_combat.BasicAttackSkill.attackCollider.OnBasicAttackPreHitEvent -= ApplyEffect;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent += RemoveMod;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent -= DisableApplyEffect;
    }

    public override void AddSubscribersToSkillEvents(Damage col)
    {
        base.AddSubscribersToSkillEvents(col);
        //proj.OnAttackApplyDamageEvent += EnableApplyEffect;
        if (false == col.IsBasicAttackCollider)
            col.OnAttackApplyDamageEvent += ApplyEffect;
    }

    public override void RemoveSubscribersToSkillEvents(Damage col)
    {
        base.RemoveSubscribersToSkillEvents(col);
        //proj.OnAttackApplyDamageEvent -= EnableApplyEffect;
        if (false == col.IsBasicAttackCollider)
            col.OnAttackApplyDamageEvent -= ApplyEffect;
    }

    private void ApplyEffect(float f)
    {
        //if (_applyEffect)
        //{
            _mod.Value = scalingStatData.FinalValue(_stats);
            _stats.AttackDamage.AddMod(_mod);
        //}
    }

    private void DisableApplyEffect(float f)
    {
        _applyEffect = false;
    }

    private void EnableApplyEffect(float f)
    {
        _applyEffect = true;
    }


    private void RemoveMod(float empty = 0)
    {
        _stats.AttackDamage.RemoveMod(_mod);
    }
}

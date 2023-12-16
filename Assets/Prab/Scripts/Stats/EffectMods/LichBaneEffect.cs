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
        _mod = new StatModifier(_scaling.FinalValue(_stats));
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackPreHitEvent += ApplyEffect;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent += RemoveMod;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent += DisableApplyEffect;
    }

    public override void DeactivateEffect()
    {
        base.DeactivateEffect();
        RemoveMod();
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackPreHitEvent -= ApplyEffect;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent += RemoveMod;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent -= DisableApplyEffect;
    }

    public override void AddSubscribersToSkillEvents(Projectile proj)
    {
        base.AddSubscribersToSkillEvents(proj);
        proj.OnAttackApplyDamageEvent += EnableApplyEffect;
    }

    public override void RemoveSubscribersToSkillEvents(Projectile proj)
    {
        base.RemoveSubscribersToSkillEvents(proj);
        proj.OnAttackApplyDamageEvent -= EnableApplyEffect;
    }

    private void ApplyEffect()
    {
        if (_applyEffect)
        {
            _mod.Value = _scaling.FinalValue(_stats);
            _stats.AttackDamage.AddMod(_mod);
        }
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

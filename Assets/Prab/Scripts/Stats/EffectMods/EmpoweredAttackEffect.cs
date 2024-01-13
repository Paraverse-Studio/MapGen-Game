using Paraverse.Mob.Stats;
using Paraverse.Stats;
using UnityEngine;


public class EmpoweredAttackEffect : MobEffect
{
    [SerializeField, Range(1, 10), Header("Empowered attack every X hit:")]
    private int _empoweredHitIndex;

    private int _hitCounter = 0;
    private StatModifier _mod;


    public override void ActivateEffect(MobStats stats)
    {
        base.ActivateEffect(stats);
        _effectNameDB = ParaverseWebsite.Models.EffectName.EmpoweredAttack;
        _mod = new StatModifier(scalingStatData.FinalValue(_stats));
        _hitCounter = 0;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackPreHitEvent += IncrementBasicAttackCounter;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent += RemoveMod;
    }

    public override void DeactivateEffect()
    {
        base.DeactivateEffect();
        RemoveMod();
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackPreHitEvent -= IncrementBasicAttackCounter;
        _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent -= RemoveMod;
    }

    private void IncrementBasicAttackCounter()
    {
        _hitCounter++;
        if (_hitCounter % _empoweredHitIndex == 0)
        {
            _mod.Value = scalingStatData.FinalValue(_stats);
            _stats.AttackDamage.AddMod(_mod);
        }
    }

    private void RemoveMod(float empty = 0)
    {
        _stats.AttackDamage.RemoveMod(_mod);
    }
}

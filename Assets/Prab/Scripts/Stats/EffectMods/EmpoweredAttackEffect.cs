using Paraverse.Mob.Stats;
using Paraverse.Stats;
using UnityEngine;


public class EmpoweredAttackEffect : MobEffect
{
  [SerializeField, Range(1, 10), Header("Empowered attack every X hit:")]
  private int[] _empoweredHitIndex;
  private StatModifier[] _mod = new StatModifier[5];
  private int _hitCounter = 0;

  public override void ActivateEffect(MobStats stats, int id, int level)
  {
    base.ActivateEffect(stats, id, level);
    _effectNameDB = ParaverseWebsite.Models.EffectName.EmpoweredAttack;
    _mod[effectLevel - 1] = new StatModifier(GetScalingStatData().FinalValue(_stats));
    _hitCounter = 0;
    _combat.BasicAttackSkill.attackCollider.OnBasicAttackPreHitEvent += IncrementBasicAttackCounter;
    _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent += RemoveMod;
  }

  public override void DeactivateEffect()
  {
    RemoveMod();
    _combat.BasicAttackSkill.attackCollider.OnBasicAttackPreHitEvent -= IncrementBasicAttackCounter;
    _combat.BasicAttackSkill.attackCollider.OnBasicAttackApplyDamageEvent -= RemoveMod;
    base.DeactivateEffect();
  }

  private void IncrementBasicAttackCounter()
  {
    _hitCounter++;
    if (_hitCounter % _empoweredHitIndex[effectLevel-1] == 0)
    {
      _mod[effectLevel - 1].Value = GetScalingStatData().FinalValue(_stats);
      _stats.AttackDamage.AddMod(_mod[effectLevel - 1]);
    }
  }

  private void RemoveMod(float empty = 0)
  {
    _stats.AttackDamage.RemoveMod(_mod[effectLevel - 1]);
  }
}

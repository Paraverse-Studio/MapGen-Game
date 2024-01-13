using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using System.Collections.Generic;
using UnityEngine;


//After killing a unit with a skill, refund 50% of the skill's cooldown (40/60/80)
public class CooldownRefundEffect : MobEffect
{
  [SerializeField, Range(0, 1)]
  protected float cooldownRefundAmount = 0.4f;

  public List<MobController> mobs;

  public override void ActivateEffect(MobStats stats)
  {
    base.ActivateEffect(stats);
    _effectNameDB = ParaverseWebsite.Models.EffectName.CooldownRefund;
    foreach (MobController enemy in EnemiesManager.Instance.Enemies)
    {
      enemy.OnDeathEvent += RefundCooldown;
    }
  }

  public override void DeactivateEffect()
  {
    base.DeactivateEffect();
    foreach (MobController enemy in EnemiesManager.Instance.Enemies)
    {
      enemy.OnDeathEvent -= RefundCooldown;
    }
  }

  public override void OnEnemyDeathApplyEffect(MobController enemy)
  {
    base.OnEnemyDeathApplyEffect(enemy);
    enemy.OnDeathEvent += RefundCooldown;
  }

  private void RefundCooldown(Transform t = null)
  {
    // dont apply effect if no active skill
    if (_combat.ActiveSkill == null) return;

    float refund = _combat.ActiveSkill.Cooldown * cooldownRefundAmount;
    _combat.ActiveSkill.RefundCooldown(refund);
  }
}

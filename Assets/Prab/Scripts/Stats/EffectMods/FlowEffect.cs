using Paraverse.Mob.Stats;
using UnityEngine;

public class FlowEffect : MobEffect
{
  public float maxFlow = 100f;
  public float curFlow = 0f;
  public bool IsMaxFlow => curFlow >= maxFlow;

  public int[] shieldAmount;
  [Range(0, 1), Tooltip("Heal amount by percentage of damage dealt")]
  public float[] healAmountRatio;

  private void ApplyHeal(float dmg)
  {
    int heal = (int)dmg * (int)healAmountRatio[effectLevel];
    _stats.UpdateCurrentHealth(heal);
    ResetFlow();
  }

  private void ApplyShield()
  {
    _stats.UpdateCurrentEnergy(shieldAmount[effectLevel]);
  }

  public override void ActivateEffect(MobStats stats)
  {
    base.ActivateEffect(stats);
  }

  public override void AddSubscribersToSkillEvents(Damage col)
  {
    base.AddSubscribersToSkillEvents(col);
    col.OnAttackApplyDamageEvent += ApplyHeal;
  }


  public override void RemoveSubscribersToSkillEvents(Damage col)
  {
    base.RemoveSubscribersToSkillEvents(col);
    col.OnAttackApplyDamageEvent -= ApplyHeal;
  }

  private void ResetFlow()
  {
    curFlow = 0f;
    // Remove shield
  }
}

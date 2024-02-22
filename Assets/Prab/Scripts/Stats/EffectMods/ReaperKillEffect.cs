using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using UnityEngine;


public class ReaperKillEffect : MobEffect
{
  [SerializeField]
  protected int[] healAmount;

  public override void ActivateEffect(MobStats stats, int id, int level)
  {
    base.ActivateEffect(stats, id, level);
    _effectNameDB = ParaverseWebsite.Models.EffectName.RepearKill;
    foreach (MobController enemy in EnemiesManager.Instance.Enemies)
    {
      enemy.OnDeathEvent += GainHealth;
    }
  }

  public override void DeactivateEffect()
  {
    foreach (MobController enemy in EnemiesManager.Instance.Enemies)
    {
      enemy.OnDeathEvent -= GainHealth;
    }
    base.DeactivateEffect();
  }

  public override void OnEnemyDeathApplyEffect(MobController enemy)
  {
    base.OnEnemyDeathApplyEffect(enemy);
    enemy.OnDeathEvent += GainHealth;
  }

  private void GainHealth(Transform t = null)
  {
    _stats.UpdateCurrentHealth(healAmount[effectLevel-1]);
  }
}

using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using UnityEngine;


//Killing a unit heals player for 3 HP (2/4/6)
public class EnergyGainEffect : MobEffect
{
    [SerializeField]
    protected int healAmount = 3;

    public override void ActivateEffect(MobStats stats)
    {
        base.ActivateEffect(stats);
        foreach (MobController enemy in EnemiesManager.Instance.Enemies)
        {
            enemy.OnDeathEvent += GainHealth;
        }
    }

    public override void DeactivateEffect()
    {
        base.DeactivateEffect();
        foreach (MobController enemy in EnemiesManager.Instance.Enemies)
        {
            enemy.OnDeathEvent -= GainHealth;
        }
    }

    private void GainHealth(Transform t = null)
    {
        _stats.UpdateCurrentHealth(healAmount);
    }
}

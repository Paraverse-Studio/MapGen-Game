using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using UnityEngine;


//After killing a unit with a skill, refund 50% of the skill's cooldown (40/60/80)
public class CooldownRefundEffect: MobEffect
{
    [SerializeField]
    protected int energyGainAmount = 20;

    public override void ActivateEffect(MobStats stats)
    {
        base.ActivateEffect(stats);
        foreach (MobController enemy in EnemiesManager.Instance.Enemies)
        {
            enemy.OnDeathEvent += GainEnergy;
        }
    }

    public override void DeactivateEffect()
    {
        base.DeactivateEffect();
        foreach (MobController enemy in EnemiesManager.Instance.Enemies)
        {
            enemy.OnDeathEvent -= GainEnergy;
        }
    }

    private void GainEnergy(Transform t = null)
    {
        _stats.UpdateCurrentEnergy(energyGainAmount);
    }
}

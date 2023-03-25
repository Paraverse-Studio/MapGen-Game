using System;
using UnityEngine;

[Serializable]
public class ScalingStatData
{
    [Header("Damage & Potency")]
    public float flatPower = 0;
    [Range(0, 3)]
    public float attackScaling = 1;
    [Range(0, 3)]
    public float abilityScaling = 0;
    [Range(0, 3)]
    public float healthScaling = 0;

    public float FinalValue(Paraverse.Mob.Stats.IMobStats stats)
    {
        float finalAttackValue = stats.AttackDamage.FinalValue * attackScaling * stats.MobBoosts.GetAttackDamageBoost();
        float finalAbilityValue = stats.AbilityPower.FinalValue * abilityScaling * stats.MobBoosts.GetAbilityPowerBoost();
        float finalHealthValue = stats.MaxHealth.FinalValue * healthScaling;

        return (flatPower + finalAttackValue + finalAbilityValue + finalHealthValue) * stats.MobBoosts.GetOverallDamageOutputBoost();
    }

}

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
        float finalAttackValue = stats.AttackDamage.FinalValue * attackScaling;
        float finalAbilityValue = stats.AbilityPower.FinalValue * abilityScaling;
        float finalHealthValue = stats.MaxHealth.FinalValue * healthScaling;

        return (flatPower + finalAttackValue + finalAbilityValue + finalHealthValue);
    }

    public float FinalValueWithBoosts(Paraverse.Mob.Stats.IMobStats stats)
    {
        float finalAttackValue = stats.AttackDamage.FinalValue * attackScaling * (null != stats.MobBoosts ? stats.MobBoosts.GetAttackDamageBoost() : 1f);
        float finalAbilityValue = stats.AbilityPower.FinalValue * abilityScaling * (null != stats.MobBoosts ? stats.MobBoosts.GetAbilityPowerBoost() : 1f);
        float finalHealthValue = stats.MaxHealth.FinalValue * healthScaling;

        return (flatPower + finalAttackValue + finalAbilityValue + finalHealthValue) * (null != stats.MobBoosts ? stats.MobBoosts.GetOverallDamageOutputBoost() : 1f);
    }

}

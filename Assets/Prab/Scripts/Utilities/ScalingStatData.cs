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
        return flatPower + 
               (stats.AttackDamage.FinalValue * attackScaling) + 
               (stats.AbilityPower.FinalValue * abilityScaling) +
               (stats.MaxHealth.FinalValue * healthScaling);
    }

}

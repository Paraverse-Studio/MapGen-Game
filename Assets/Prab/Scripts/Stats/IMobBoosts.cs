using Paraverse.Stats;

namespace Paraverse.Mob.Boosts
{
    public interface IMobBoosts 
    {
        public Stat OverallDamageBoost { get; }
        public Stat AttackDamageBoost { get; }
        public Stat AbilityPowerBoost { get; }

        public float GetOverallDamageOutputBoost();
        public float GetAttackDamageBoost();
        public float GetAbilityPowerBoost();
    }
}

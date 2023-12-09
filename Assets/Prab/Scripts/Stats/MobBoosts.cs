using Paraverse.Stats;
using UnityEngine;

/* Mob boosts class is similar to Mob Stats class but it's separated to have less clutter in Mob Stats.
 * Mob boosts are overall %-based boosts applied to overall damage, or to overall attack or ability.
 * This is mainly to be used for Effect mods, particularly "buffs" 
 */
namespace Paraverse.Mob.Boosts
{
    public class MobBoosts : MonoBehaviour, IMobBoosts
    {
        [Header("Overall boosts applied to stats/damage")]
        #region Variables
        [SerializeField]
        protected int overallDamageBoost = 0;
        public Stat OverallDamageBoost { get { return _overallDamageBoost; } }
        private Stat _overallDamageBoost;

        [SerializeField]
        protected int attackDamageBoost = 0;
        public Stat AttackDamageBoost { get { return _attackDamageBoost; } }
        private Stat _attackDamageBoost;

        [SerializeField]
        protected int abilityPowerBoost = 0;
        public Stat AbilityPowerBoost { get { return _abilityPowerBoost; } }
        private Stat _abilityPowerBoost;
        #endregion

        protected void Awake()
        {
            _overallDamageBoost = new Stat(overallDamageBoost);
            _attackDamageBoost = new Stat(attackDamageBoost);
            _abilityPowerBoost = new Stat(abilityPowerBoost);
        }

        // Use this for all damage output, the overall boost to it
        public float GetOverallDamageOutputBoost()
        {
            return Mathf.Max(1, OverallDamageBoost.FinalValue);
        }

        // Use this for skills or attacks that scale with attack
        public float GetAttackDamageBoost()
        {
            return Mathf.Max(1, AttackDamageBoost.FinalValue);
        }

        // Use this for skills that scale with ability
        public float GetAbilityPowerBoost()
        {
            return Mathf.Max(1, AbilityPowerBoost.FinalValue);
        }

    }

}
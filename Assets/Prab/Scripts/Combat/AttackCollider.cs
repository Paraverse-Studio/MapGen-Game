using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;

namespace Paraverse
{
    public class AttackCollider : Damage, IDamage
    {
        public override void Init(MobCombat mob, ScalingStatData scalingStatData)
        {
            this.mob = mob;
            this.scalingStatData.flatPower = scalingStatData.flatPower;
            this.scalingStatData.attackScaling = scalingStatData.attackScaling;
            this.scalingStatData.abilityScaling = scalingStatData.abilityScaling;
            gameObject.SetActive(false);
        }

        public override void Init(MobCombat mob)
        {
            this.mob = mob;
            gameObject.SetActive(false);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (dot == false) return;

            if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject) && applyHit)
            {
                DamageLogic(other);
                dotTimer = dotIntervalTimer;
                hitTargets.Add(other.gameObject);
                applyHit = false;

                Debug.Log(other.name + " took " + mob.stats.AttackDamage.FinalValue + " points of damage.");
            }
        }

        /// <summary>
        /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
        /// </summary>
        protected override float ApplyCustomDamage(IMobController controller)
        {
            float totalDmg = scalingStatData.FinalValueWithBoosts((IMobStats)mob.stats);

            controller.Stats.UpdateCurrentHealth(-Mathf.CeilToInt(totalDmg));
            return totalDmg;
        }
    }
}
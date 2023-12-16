using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace Paraverse
{
    public class AttackCollider : Damage, IDamage
    {
        private List<GameObject> hitTargets = new List<GameObject>();

        private bool applyHit = false;

        
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

        private void OnEnable()
        {
            hitTargets.Clear();
        }

        protected override void Update()
        {
            if (dotTimer <= 0)
            {
                applyHit = true;
                hitTargets.Clear();
                dotTimer = dotIntervalTimer;
            }
            else
                dotTimer -= Time.deltaTime;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (dontApplyDamageOnEnter == true) return;

            // Detecting type of object/enemy hit
            if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject))
            {
                DamageLogic(other);
            }
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

        protected override void DamageLogic(Collider other)
        {
            InvokePreHitEvent();

            hitTargets.Add(other.gameObject);

            // Enemy-related logic
            if (other.TryGetComponent(out IMobController controller))
            {
                // Apply damage
                float dmg = ApplyCustomDamage(controller);

                InvokeApplyDamageEvent(dmg);

                // Apply knock back effect
                if (null != knockBackEffect)
                {
                    KnockBackEffect effect = new KnockBackEffect(knockBackEffect);
                    controller.ApplyKnockBack(mob.transform.position, effect);
                }
                else if (applyHitAnim)
                {
                    controller.ApplyHitAnimation();
                }
            }

            // General VFX logic
            if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

            InvokePostHitEvent();

            Debug.Log(other.name + " took " + scalingStatData.FinalValue(mob.stats) + " points of damage.");
        }
    }
}
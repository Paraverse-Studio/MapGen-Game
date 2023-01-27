using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Paraverse
{
    public class AttackCollider : MonoBehaviour
    {
        private MobCombat mob;
        private IMobStats stats;
        [SerializeField, Tooltip("Enter the tag of target.")]
        private string targetTag = "Player";
        private List<GameObject> hitTargets = new List<GameObject>();

        [SerializeField, Tooltip("Set turn for damage over time.")]
        private bool dot = false;
        private float timer = 0f;
        private bool applyHit = false;
        private float attackPerUnitOfTime = 1f;

        [Header("Knockback Effect")]
        [SerializeField]
        private KnockBackEffect knockBackEffect;

        [Header("VFX")]
        //public GameObject launchFX;
        public GameObject hitFX;

        [SerializeField]
        protected bool isBasicAttackCollider = false;
        private bool isSkill = false;

        public delegate void OnBasicAttackLandPreDmgDel();
        public event OnBasicAttackLandPreDmgDel OnBasicAttackPreHitEvent;
        public delegate void OnBasicAttackApplyDamageDel(float dmg);
        public event OnBasicAttackApplyDamageDel OnBasicAttackApplyDamageEvent;
        public delegate void OnBasicAttackLandPostDmgDel();
        public event OnBasicAttackLandPostDmgDel OnBasicAttackPostHitEvent;

        // Updated via Mob Skill
        [HideInInspector]
        public ScalingStatData scalingStatData;


        public void Init(MobCombat mob, IMobStats stats, ScalingStatData scalingStatData, bool isSkill = false)
        {
            this.mob = mob;
            this.stats = stats;
            this.scalingStatData.flatPower = scalingStatData.flatPower;
            this.scalingStatData.attackScaling = scalingStatData.attackScaling;
            this.scalingStatData.abilityScaling = scalingStatData.abilityScaling;
            this.isSkill = isSkill;
        }

        private void OnEnable()
        {
            hitTargets.Clear();
        }

        private void Update()
        {
            if (timer <= 0)
            {
                applyHit = true;
                hitTargets.Clear();
                timer = attackPerUnitOfTime;
            }
            else
                timer -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (dot) return;

            // Detecting type of object/enemy hit
            if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject))
            {
                // Pre Basic Attack Hit Event
                if (isBasicAttackCollider)
                    OnBasicAttackPreHitEvent?.Invoke();
                
                hitTargets.Add(other.gameObject);

                // Enemy-related logic
                if (other.TryGetComponent(out IMobController controller))
                {
                    // Apply damage
                    float dmg = ApplyCustomDamage(controller);


                    // On Damage Applied Event
                    if (isBasicAttackCollider)
                        OnBasicAttackApplyDamageEvent?.Invoke(dmg);

                    // Apply knock back effect
                    if (null != knockBackEffect)
                    {
                        KnockBackEffect effect = new KnockBackEffect(knockBackEffect);
                        controller.ApplyKnockBack(mob.transform.position, effect);
                    }
                }

                // General VFX logic
                if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

                // Post Basic Attack Hit Event
                if (isBasicAttackCollider)
                    OnBasicAttackPostHitEvent?.Invoke();

                Debug.Log(other.name + " took " + stats.AttackDamage.FinalValue + " points of damage.");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (dot == false) return;

            if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject) && applyHit)
            {
                hitTargets.Add(other.gameObject);

                IMobController controller = other.GetComponent<IMobController>();
                controller.Stats.UpdateCurrentHealth((int)-stats.AttackDamage.FinalValue);
                controller.ApplyKnockBack(mob.transform.position, knockBackEffect);
                applyHit = false;
                timer = attackPerUnitOfTime;

                Debug.Log(other.name + " took " + stats.AttackDamage.FinalValue + " points of damage.");
            }
        }

        /// <summary>
        /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
        /// </summary>
        public float ApplyCustomDamage(IMobController controller)
        {
            float phyDmg = controller.Stats.AttackDamage.FinalValue * scalingStatData.flatPower;
            float abilityDmg = controller.Stats.AbilityPower.FinalValue;
            float totalDmg = phyDmg;

            phyDmg += phyDmg * scalingStatData.attackScaling;
            abilityDmg += abilityDmg * scalingStatData.abilityScaling;

            if (isSkill)
            {
                totalDmg = phyDmg + abilityDmg;
            }

            controller.Stats.UpdateCurrentHealth((int)-totalDmg);
            return totalDmg;
        }
    }
}
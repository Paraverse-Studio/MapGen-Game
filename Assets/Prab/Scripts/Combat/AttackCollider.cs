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
        [SerializeField, Tooltip("Apply damage upon enter.")]
        protected bool dontApplyDamageOnEnter = false;
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
        
        public void Init(MobCombat mob, IMobStats stats)
        {
            this.mob = mob;
            this.stats = stats;
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
            if (dontApplyDamageOnEnter == true) return;

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
                OnTriggerEnter(other);
                timer = attackPerUnitOfTime;
                hitTargets.Add(other.gameObject);
                applyHit = false;

                Debug.Log(other.name + " took " + stats.AttackDamage.FinalValue + " points of damage.");
            }
        }

        /// <summary>
        /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
        /// </summary>
        public float ApplyCustomDamage(IMobController controller)
        {
            float totalDmg =
                scalingStatData.flatPower + 
                (stats.AttackDamage.FinalValue * scalingStatData.attackScaling) + 
                (stats.AbilityPower.FinalValue * scalingStatData.abilityScaling);            

            controller.Stats.UpdateCurrentHealth(-Mathf.CeilToInt(totalDmg));
            return totalDmg;
        }
    }
}
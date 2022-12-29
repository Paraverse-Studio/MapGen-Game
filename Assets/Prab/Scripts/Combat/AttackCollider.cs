using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using System.Collections.Generic;
using UnityEngine;

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
            if (dot) return;

            // Detecting type of object/enemy hit
            if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject))
            {
                hitTargets.Add(other.gameObject);

                // Enemy-related logic
                IMobController controller;
                if (other.TryGetComponent(out controller))
                {
                    controller.Stats.UpdateCurrentHealth((int)-stats.AttackDamage.FinalValue);
                    
                    // Apply knock back effect
                    if (null != knockBackEffect)
                    {
                        KnockBackEffect effect = new KnockBackEffect(knockBackEffect);
                        controller.ApplyKnockBack(mob.transform.position, effect);
                    }
                }

                // General VFX logic
                if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

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
    }
}
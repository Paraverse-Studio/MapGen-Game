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
            if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject))
            {
                hitTargets.Add(other.gameObject);

                IMobController controller = other.GetComponent<IMobController>();
                controller.Stats.UpdateCurrentHealth((int)-stats.AttackDamage.FinalValue);
                controller.ApplyKnockBack(mob.transform.position);

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
                controller.ApplyKnockBack(mob.transform.position);
                applyHit = false;
                timer = attackPerUnitOfTime;

                Debug.Log(other.name + " took " + stats.AttackDamage.FinalValue + " points of damage.");
            }
        }
    }
}
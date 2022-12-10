using Paraverse.Mob;
using Paraverse.Mob.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace Paraverse
{
    public class AttackCollider : MonoBehaviour
    {
        private MobCombat mob;
        [SerializeField, Tooltip("Enter the tag of target.")]
        private string targetTag = "Player";
        private float damage;
        private List<GameObject> hitTargets = new List<GameObject>();


        public void Init(MobCombat mob, float damage)
        {
            this.mob = mob;
            this.damage = damage;
        }

        private void OnEnable()
        {
            hitTargets.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject))
            {
                //ContactPoint contact = other.contacts[0];
                hitTargets.Add(other.gameObject);

                IMobController controller = other.GetComponent<IMobController>();
                controller.Stats.UpdateCurrentHealth((int)-damage);
                controller.ApplyKnockBack(mob.transform.position);

                Debug.Log(other.name + " took " + damage + " points of damage.");
            }
        }
    }
}
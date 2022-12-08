using Paraverse.Mob;
using Paraverse.Mob.Combat;
using UnityEngine;

namespace Paraverse
{
    public class AttackCollider : MonoBehaviour
    {
        private MobCombat mob;
        [SerializeField, Tooltip("Enter the tag of target.")]
        private string targetTag = "Player";
        private float damage;


        public void Init(MobCombat mob, float damage)
        {
            this.mob = mob;
            this.damage = damage;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag))
            {
                //ContactPoint contact = other.contacts[0];

                IMobController controller = other.GetComponent<IMobController>();
                controller.Stats.UpdateCurrentHealth((int)-damage);
                controller.ApplyKnockBack(mob.transform.position);

                Debug.Log(other.name + " took " + damage + " points of damage.");
            }
        }
    }
}
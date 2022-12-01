using Paraverse.Mob.Stats;
using UnityEngine;

namespace Paraverse
{
    public class AttackCollider : MonoBehaviour
    {
        [SerializeField, Tooltip("Enter the tag of target.")]
        private string targetTag = "Player";
        private float damage;


        public void Init(float damage)
        {
            this.damage = damage;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag))
            {
                other.GetComponent<IMobStats>().UpdateCurrentHealth(-damage);
                Debug.Log(other.name + " took " + damage + " points of damage.");
            }
        }
    }
}
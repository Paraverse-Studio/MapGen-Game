using Paraverse.Helper;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;

namespace Paraverse
{
    public class Projectile : MonoBehaviour
    {
        #region Variables
        private MobCombat mob;
        [SerializeField, Tooltip("Speed of the projectile.")]
        private string targetTag = "Player";
        private Vector3 target;
        [SerializeField, Tooltip("Speed of the projectile.")]
        private float speed;
        [SerializeField, Tooltip("Range of the projectile.")]
        private float range;
        [SerializeField, Tooltip("Range of the projectile.")]
        private float damage;
        [SerializeField, Tooltip("Projectile is destroyed after this duration.")]
        private float deathTimer = 5f;

        [Header("Special Properties")]
        public bool pierce = false;

        private float curdeathTimer = 0f;
        private Vector3 origin;
        #endregion

        #region Start & Update
        private void Start()
        {
            origin = transform.position;
        }

        private void Update()
        {
            float distanceFromOrigin = ParaverseHelper.GetDistance(transform.position, origin);
            if (distanceFromOrigin > range || curdeathTimer >= deathTimer)
            {
                Destroy(gameObject);
            }

            curdeathTimer += Time.deltaTime;
            transform.position += (target * speed * Time.deltaTime);
        }
        #endregion

        public void Init(MobCombat mob, Vector3 target, float speed, float range, float damage)
        {
            this.target = target;
            this.mob = mob;
            this.speed = speed;
            this.range = range;
            this.damage = damage;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag))
            {
                IMobController controller = other.GetComponent<IMobController>();
                controller.Stats.UpdateCurrentHealth((int)-damage);
                controller.ApplyKnockBack(mob.transform.position);
                if (!pierce) Destroy(gameObject);
            }
        }
    }
}
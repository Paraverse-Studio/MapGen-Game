using Paraverse.Helper;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using UnityEngine;

namespace Paraverse
{
    public class Projectile : MonoBehaviour
    {
        #region Variables
        private MobCombat mob;
        [SerializeField, Tooltip("Speed of the projectile.")]
        private string targetTag = "Player";
        private Vector3 target = Vector3.forward;
        [SerializeField, Tooltip("Speed of the projectile.")]
        private float speed;
        [SerializeField, Tooltip("Range of the projectile.")]
        private float range;
        [SerializeField, Tooltip("Range of the projectile.")]
        private float damage;
        [SerializeField, Tooltip("Projectile is destroyed after this duration.")]
        private float deathTimer = 5f;
        [SerializeField]
        protected bool stationary = false;

        [Header("Knockback Effect")]
        [SerializeField]
        private KnockBackEffect knockBackEffect;

        [Header("Special Properties")]
        public bool pierce = false;

        [Header("VFX")]
        public GameObject launchFX;
        public GameObject hitFX;

        private float curdeathTimer = 0f;
        private Vector3 origin;
        #endregion

        #region Start & Update
        private void Start()
        {
            origin = transform.position;

            if (launchFX) Instantiate(hitFX, origin, Quaternion.identity);
        }

        private void Update()
        {
            float distanceFromOrigin = ParaverseHelper.GetDistance(transform.position, origin);
            if (distanceFromOrigin > range && stationary == false || curdeathTimer >= deathTimer)
            {
                Destroy(gameObject);
            }

            curdeathTimer += Time.deltaTime;

            if (stationary == false) transform.position += (transform.forward * speed * Time.deltaTime);
        }
        #endregion

        public void Init(MobCombat mob, Vector3 target, float damage)
        {
            this.target = target;
            this.mob = mob;
            this.damage = damage;
        }

        public void Init(MobCombat mob, Vector3 target, float range, float damage)
        {
            this.target = target;
            this.mob = mob;
            this.range = range;
            this.damage = damage;
        }

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

                // Apply knock back effect
                if (null != knockBackEffect)
                {
                    KnockBackEffect effect = new KnockBackEffect(knockBackEffect);
                    controller.ApplyKnockBack(mob.transform.position, effect);
                }

                if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

                if (!pierce) Destroy(gameObject);
            }
        }
    }
}
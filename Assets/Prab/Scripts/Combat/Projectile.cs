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
        [SerializeField, Tooltip("Stationary projectile.")]
        protected bool stationary = false;
        [SerializeField, Tooltip("Damage over time.")]
        protected bool dot = false;
        [SerializeField, Tooltip("Apply damage upon enter.")]
        protected bool dontApplyDamageOnEnter = false;
        [SerializeField, Tooltip("Applies damage every given second.")]
        protected float dotIntervalTimer = 1f;
        private float dotTimer = 0f;

        [Header("Knockback Effect")]
        [SerializeField]
        private KnockBackEffect knockBackEffect;

        [Header("Special Properties")]
        [SerializeField, Tooltip("Projectile is not destroyed upon impact.")]
        protected bool pierce = false;
        [SerializeField, Tooltip("Applies hit animation to target.")]
        protected bool applyHitAnim = true;
        [SerializeField, Tooltip("The projectile is a beam.")]
        protected bool isBeam = false;

        [Header("VFX")]
        public GameObject launchFX;
        public GameObject hitFX;

        [Header("Motion")]
        public float decreaseByLerp = -1f;

        private float curdeathTimer = 0f;
        private Vector3 origin;

        public ScalingStatData scalingStatData;
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

            dotTimer += Time.deltaTime;
            curdeathTimer += Time.deltaTime;

            if (stationary == false)
            {
                transform.position += transform.forward * speed * Time.deltaTime;
                if (decreaseByLerp > 0) speed = Mathf.Lerp(speed, 0, decreaseByLerp * Time.deltaTime);
            }
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

        protected void OnTriggerEnter(Collider other)
        {
            if (dontApplyDamageOnEnter == true) return;

            if (other.CompareTag(targetTag))
            {
                DamageLogic(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(targetTag) && dotTimer >= dotIntervalTimer)
            {
                dotTimer = 0f;
                DamageLogic(other);
            }
        }

        /// <summary>
        /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
        /// </summary>
        public float ApplyCustomDamage(IMobController controller)
        {
            float totalDmg =
                scalingStatData.flatPower +
                (controller.Stats.AttackDamage.FinalValue * scalingStatData.attackScaling) +
                (controller.Stats.AbilityPower.FinalValue * scalingStatData.abilityScaling);

            controller.Stats.UpdateCurrentHealth(-Mathf.CeilToInt(totalDmg));
            return totalDmg;
        }

        private void DamageLogic(Collider other)
        {
            IMobController controller = other.GetComponent<IMobController>();
            if (null != controller)
            {
                ApplyCustomDamage(controller);

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

            if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

            if (!pierce) Destroy(gameObject);
        }
    }
}
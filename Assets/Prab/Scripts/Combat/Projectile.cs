using Paraverse.Helper;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace Paraverse
{
    public class Projectile : MonoBehaviour
    {
        #region Variables
        protected MobCombat mob;

        [Header("Projectile Stats")]
        [SerializeField, Tooltip("Speed of the projectile.")]
        protected string targetTag = "Player";
        protected Vector3 target = Vector3.forward;
        [SerializeField, Tooltip("Speed of the projectile.")]
        protected float speed = 20f;
        [SerializeField, Tooltip("Range of the projectile.")]
        protected float range = 10f;
        [SerializeField, Tooltip("Projectile is destroyed after this duration.")]
        protected float deathTimer = 5f;

        [Header("Projectile Properties")]
        [SerializeField, Tooltip("Stationary projectile.")]
        protected bool stationary = false;
        [SerializeField, Tooltip("Damage over time.")]
        protected bool dot = false;
        [SerializeField, Tooltip("Apply damage upon enter.")]
        protected bool dontApplyDamageOnEnter = false;
        [SerializeField, Tooltip("Applies damage every given second.")]
        protected float dotIntervalTimer = 1f;
        protected float dotTimer = 0f;

        [Header("Knockback Effect")]
        [SerializeField]
        protected KnockBackEffect knockBackEffect;

        [Header("Special Properties")]
        [SerializeField, Tooltip("Projectile is not destroyed upon impact.")]
        protected bool pierce = false;
        [SerializeField, Tooltip("Applies hit animation to target.")]
        protected bool applyHitAnim = true;

        [Header("VFX")]
        public GameObject launchFX;
        public GameObject hitFX;

        [Header("Motion")]
        public float decreaseByLerp = -1f;

        protected float curdeathTimer = 0f;
        protected Vector3 origin;

        protected ScalingStatData scalingStatData;
        #endregion

        #region Start & Update
        protected virtual void Start()
        {
            origin = transform.position;

            if (launchFX) Instantiate(hitFX, origin, Quaternion.identity);
        }

        protected virtual void Update()
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

        public virtual void Init(MobCombat mob, Vector3 target, ScalingStatData statData)
        {
            Debug.Log("GOT THIS: " + statData.flatPower);

            this.target = target;
            this.mob = mob;
            scalingStatData = statData;
            Debug.Log("NOW ITS THIS: " + scalingStatData.flatPower);

        }

        public virtual void Init(MobCombat mob, Vector3 target, float speed, ScalingStatData statData)
        {
            this.target = target;
            this.mob = mob;
            this.speed = speed;
            scalingStatData = statData;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (dontApplyDamageOnEnter == true) return;

            if (other.CompareTag(targetTag))
            {
                DamageLogic(other);
            }
        }

        protected virtual void OnTriggerStay(Collider other)
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
        protected void ApplyCustomDamage(IMobController controller)
        {
            controller.Stats.UpdateCurrentHealth(-Mathf.CeilToInt(scalingStatData.FinalValue(mob.stats)));
        }

        protected void DamageLogic(Collider other)
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
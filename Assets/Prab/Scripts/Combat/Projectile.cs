using Paraverse.Helper;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;

namespace Paraverse
{
    public class Projectile : Damage, IDamage
    {
        #region Variables

        [Header("Projectile Stats")]
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


        [Header("Special Properties")]
        [SerializeField, Tooltip("Projectile is not destroyed upon impact.")]
        protected bool pierce = false;

        [Header("Motion")]
        public float decreaseByLerp = -1f;

        protected float curdeathTimer = 0f;
        protected Vector3 origin;

        public GameObject launchFX;

        #endregion

        #region Start & Update
        protected override void Start()
        {
            origin = transform.position;

            if (launchFX) Instantiate(hitFX, origin, Quaternion.identity);
        }

        protected override void Update()
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
                transform.position += ParaverseHelper.GetPositionXZ(target) * speed * Time.deltaTime;
                if (decreaseByLerp > 0) speed = Mathf.Lerp(speed, 0, decreaseByLerp * Time.deltaTime);
            }
        }
        #endregion

        public virtual void Init(MobCombat mob, Vector3 target, ScalingStatData statData)
        {
            this.target = target;
            this.mob = mob;
            scalingStatData = statData;
        }

        public virtual void Init(MobCombat mob, Vector3 target, float speed, ScalingStatData statData)
        {
            this.target = target;
            this.mob = mob;
            this.speed = speed;
            scalingStatData = statData;
        }

        protected override void DamageLogic(Collider other)
        {
            base.DamageLogic(other);

            if (!pierce) Destroy(gameObject);
        }
    }
}
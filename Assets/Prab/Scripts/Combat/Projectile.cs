using Paraverse.Helper;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Player;
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
                transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * speed * Time.deltaTime;
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

        protected virtual void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject) && applyHit && dotTimer >= dotIntervalTimer && dot)
            {
                DamageLogic(other);
                dotTimer = dotIntervalTimer;
                hitTargets.Add(other.gameObject);
                applyHit = false;

                Debug.Log(other.name + " took " + mob.stats.AttackDamage.FinalValue + " points of damage.");
            }
        }

        /// <summary>
        /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
        /// </summary>
        protected override float ApplyCustomDamage(IMobController controller)
        {
            float totalDmg = Mathf.CeilToInt(scalingStatData.FinalValue(mob.stats));
            controller.Stats.UpdateCurrentHealth(-(int)totalDmg);
            Debug.Log("Applied " + totalDmg + " points of damage to " + controller.Transform.name);
            return totalDmg;
        }

        protected override void DamageLogic(Collider other)
        {
            base.DamageLogic(other);

            if (!pierce) Destroy(gameObject);
        }
    }
}
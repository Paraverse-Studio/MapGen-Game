using Paraverse.Helper;
using Paraverse.Mob.Stats;
using UnityEngine;

namespace Paraverse.Mob.Combat
{
    public class MobCombat : MonoBehaviour, IMobCombat
    {
        #region Variables
        // Important unity components
        [SerializeField]
        protected Animator anim;

        // Required reference scripts
        protected IMobStats stats;

        [Header("Target Values")]
        [Tooltip("Target tag (Player by default).")]
        protected string targetTag = "Player";
        protected Transform player;

        [Header("Basic Attack Values")]
        [SerializeField, Range(0, 1), Tooltip("Basic attack damage ratio of attack damage stat.")]
        protected float basicAtkDmgRatio = 1f;
        [SerializeField, Tooltip("Basic attack range.")]
        protected float basicAtkRange = 2f;
        public float BasicAtkRange { get { return basicAtkRange; } }

        [Header("Basic Attack Speed Values")]
        [SerializeField, Range(0.5f, 1f)]
        protected float minBasicAtkAnimSpeed = 0.8f;
        [SerializeField, Range(1f, 2.5f)]
        protected float maxBasicAtkAnimSpeed = 2f;
        protected float curBasicAtkAnimSpeed;
        protected float curBasicAtkCd;

        [Header("Only For Melee Attackers")]
        [SerializeField, Tooltip("Basic attack weapon collider [Only required for melee weapon users].")]
        private GameObject basicAtkCollider;
        [Tooltip("AttackCollider script of basic attack collider.")]
        private AttackCollider basicAtkColScript;

        [Header("Projectile Values")]
        [SerializeField, Tooltip("Set as true if mob is a projectile user.")]
        protected bool projUser = false;
        [SerializeField, Tooltip("Basic attack projectile prefab.")]
        protected GameObject projPf;
        [SerializeField, Tooltip("The projectile object held by the mob.")]
        protected GameObject projHeld;
        [SerializeField, Tooltip("The projectile's origin position.")]
        protected Transform projOrigin;
        [SerializeField, Tooltip("The projectile's speed.")]
        protected float basicAtkProjSpeed = 10f;

        // Constantly updates the distance from player
        protected float distanceFromTarget;

        // Sets to true when character is doing an action (Attack, Stun).
        protected bool isInteracting = false;
        public bool IsInteracting { get { return isInteracting; } }

        // Returns true when character is within basic attack range and cooldown is 0.
        public bool CanBasicAtk { get { return distanceFromTarget <= basicAtkRange && curBasicAtkCd <= 0; } }
        #endregion

        #region Start & Update Methods
        protected virtual void Start()
        {
            if (anim == null) anim = GetComponent<Animator>();
            if (player == null) player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
            if (stats == null) stats = GetComponent<IMobStats>();

            Initialize();
        }

        protected virtual void Update()
        {
            distanceFromTarget = ParaverseHelper.GetDistance(transform.position, player.position);
            AttackCooldownHandler();
            EnergyHandler();

            // used to control player movement - if isInteracting => curSpeed = 0f
            isInteracting = anim.GetBool(StringData.IsInteracting);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Initializes player on Start() method.
        /// </summary>
        protected virtual void Initialize()
        {
            // Gets distance from target on start
            distanceFromTarget = ParaverseHelper.GetDistance(transform.position, player.position);

            // Gets and initiates attack collider script on the enemy 
            if (projUser == false)
            {
                // Checks if melee users have basic attack collider script on weapon
                if (basicAtkCollider == null)
                {
                    Debug.LogError(gameObject.name + " needs to have a basic attack collider!");
                    return;
                }
                basicAtkCollider.SetActive(true);
                basicAtkColScript = basicAtkCollider.GetComponent<AttackCollider>();
                basicAtkColScript.Init(this, basicAtkDmgRatio * stats.AttackDamage);
                basicAtkCollider.SetActive(false);
            }
        }
        #endregion

        #region Basic Attack Logic
        /// <summary>
        /// Responsible for handling basic attack animation and cooldown.
        /// </summary>
        public virtual void BasicAttackHandler()
        {
            if (curBasicAtkCd <= 0)
            {
                anim.Play(StringData.BasicAttack);
                curBasicAtkCd = GetBasicAttackCooldown();
                //Debug.Log("Basic attack");
            }
        }

        /// <summary>
        /// Handles basic attack cooldowns
        /// </summary>
        protected virtual void AttackCooldownHandler()
        {
            curBasicAtkCd -= Time.deltaTime;
            curBasicAtkCd = Mathf.Clamp(curBasicAtkCd, 0f, GetBasicAttackCooldown());

            // Updates the mobs anim attack speed
            SetBasicAttackAnimSpeed();
        }

        /// <summary>
        /// Returns the basic attack cooldown based on attack speed value in stats.
        /// </summary>
        /// <returns></returns>
        protected float GetBasicAttackCooldown()
        {
            return 1f / stats.AttackSpeed;
        }

        /// <summary>
        /// Sets the attack animation speed and returns the basic attack cooldown based on the attack speed stat value.
        /// </summary>
        /// <returns></returns>
        protected void SetBasicAttackAnimSpeed()
        {
            // Updates the attack animation speed 
            curBasicAtkAnimSpeed = stats.AttackSpeed;
            curBasicAtkAnimSpeed = Mathf.Clamp(curBasicAtkAnimSpeed, minBasicAtkAnimSpeed, maxBasicAtkAnimSpeed);
            anim.SetFloat(StringData.AttackSpeed, curBasicAtkAnimSpeed);
            //Debug.Log(transform.name + " attacks " + stats.AttackSpeed + " times per second.");
            //Debug.Log(transform.name + "'s animation speed is " + anim.GetFloat(StringData.AttackSpeed) + " seconds.");
        }
        #endregion

        #region Energy Handler
        /// <summary>
        /// Handles basic attack cooldowns
        /// </summary>
        protected virtual void EnergyHandler()
        {
            stats.UpdateCurrentEnergy(stats.EnergyRegen * Time.deltaTime);            
        }
        #endregion

        #region Animation Event Methods
        /// <summary>
        /// Enables basic weapon collider.
        /// </summary>
        protected void EnableBasicAttackCollider()
        {
            basicAtkCollider.SetActive(true);
        }

        /// <summary>
        /// Disables basic attack collider.
        /// </summary>
        protected void DisableBasicAttackCollider()
        {
            basicAtkCollider.SetActive(false);
        }

        /// <summary>
        /// Fires a projectile and disables the projectile held by the mob (ONLY if mob is holding a proj).
        /// </summary>
        public void FireProjectile()
        {
            // Archers may hold an arrow which needs to be set to off/on when firing
            if (projHeld != null)
                projHeld.SetActive(false);

            // Instantiate and initialize projectile
            GameObject go = Instantiate(projPf, projOrigin.position, transform.rotation);
            Projectile proj = go.GetComponent<Projectile>();
            proj.Init(basicAtkProjSpeed, basicAtkRange, basicAtkDmgRatio * stats.AttackDamage);
        }

        /// <summary>
        /// Enables the projectile held by the mob.
        /// </summary>
        public void EnableHeldProjectile()
        {
            if (projHeld == null)
            {
                Debug.LogError("There is no reference to the projHeld variable.");
                return;
            }

            projHeld.SetActive(true);
        }
        #endregion
    }
}
using Paraverse.Helper;
using Paraverse.Mob.Stats;
using System;
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
        [HideInInspector]
        public MobStats stats;
        protected IMobController controller;

        [Header("Target Values")]
        [Tooltip("Target tag (Player by default).")]
        protected string targetTag = "Player";
        protected Transform player;

        [Header("Basic Attack Values")]
        [SerializeField, Range(0, 1), Tooltip("Basic attack damage ratio of attack damage stat.")]
        protected float basicAtkDmgRatio = 1f;
        protected float curBasicAtkCd;
        [SerializeField, Tooltip("Basic attack range.")]
        protected float basicAtkRange = 2f;

        [Header("Only For Melee Attackers")]
        [SerializeField, Tooltip("Basic attack weapon collider [Only required for melee weapon users].")]
        protected GameObject basicAttackColliderGO;
        [Tooltip("AttackCollider script of basic attack collider.")]
        public AttackCollider basicAttackCollider;

        [Header("Projectile Values")]
        [SerializeField, Tooltip("Set as true if mob is a projectile user.")]
        protected bool projUser = false;
        [SerializeField]
        protected ProjectileData projData;

        // Constantly updates the distance from player
        protected float distanceFromTarget;
        public Transform Target { get { return _target; } set { _target = value; } }
        private Transform _target;

        public float BasicAtkRange { get { return basicAtkRange; } }
        public bool IsBasicAttacking { get { return _isBasicAttacking; } }
        protected bool _isBasicAttacking = false;
        // Returns true when character is within basic attack range and cooldown is 0.
        public bool CanBasicAtk { get { return distanceFromTarget <= basicAtkRange && curBasicAtkCd <= 0; } }
        // Sets to true when character is doing an action (Attack, Stun).
        public bool IsAttackLunging { get { return _isAttackLunging; } }
        protected bool _isAttackLunging = false;
        public bool IsSkilling { get; set; }
        public bool IsInCombat { get { return IsSkilling || IsBasicAttacking; } }
        #endregion


        #region Start & Update Methods
        protected virtual void Start()
        {
            if (anim == null) anim = GetComponent<Animator>();
            if (player == null) player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
            if (player != null) _target = player;
            if (stats == null) stats = GetComponent<MobStats>();
            if (controller == null) controller = GetComponent<IMobController>();

            Initialize();
        }

        protected virtual void Update()
        {
            if (controller.IsDead) return;

            distanceFromTarget = ParaverseHelper.GetDistance(transform.position, player.position);

            _isBasicAttacking = anim.GetBool(StringData.IsBasicAttacking);
            AttackCooldownHandler();
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
                if (null == basicAttackColliderGO)
                {
                    Debug.LogWarning(gameObject.name + " needs to have a basic attack collider!");
                    return;
                }
                basicAttackColliderGO.SetActive(true);
                basicAttackCollider = basicAttackColliderGO.GetComponent<AttackCollider>();
                if (gameObject.CompareTag(StringData.PlayerTag))
                {
                    basicAttackCollider.Init(this, stats);
                }
                else
                {
                    basicAttackCollider.Init(this, stats, projData.scalingStatData);
                }
                basicAttackColliderGO.SetActive(false);
            }
        }
        #endregion

        #region Basic Attack Logic
        /// <summary>
        /// Handles basic attack cooldowns
        /// </summary>
        protected virtual void AttackCooldownHandler()
        {
            curBasicAtkCd -= Time.deltaTime;
            curBasicAtkCd = Mathf.Clamp(curBasicAtkCd, 0f, GetBasicAttackCooldown());
        }

        /// <summary>
        /// Returns the basic attack cooldown based on attack speed value in stats.
        /// </summary>
        /// <returns></returns>
        protected float GetBasicAttackCooldown()
        {
            return 1f / stats.AttackSpeed.FinalValue;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Responsible for handling basic attack animation and cooldown.
        /// </summary>
        public virtual void BasicAttackHandler()
        {
            if (curBasicAtkCd <= 0)
            {
                anim.Play(StringData.BasicAttack);
                curBasicAtkCd = GetBasicAttackCooldown();
            }
        }

        /// <summary>
        /// Resets booelans when mob is interrupted during attack. 
        /// </summary>
        public void OnAttackInterrupt()
        {
            _isAttackLunging = false;
            DisableBasicAttackCollider();
            IsSkilling = false;
        }
        #endregion

        #region Animation Event Methods
        /// <summary>
        /// Enables basic weapon collider.
        /// </summary>
        protected void EnableBasicAttackCollider()
        {
            if (basicAttackColliderGO != null)
                basicAttackColliderGO.SetActive(true);
        }

        /// <summary>
        /// Disables basic attack collider.
        /// </summary>
        protected void DisableBasicAttackCollider()
        {
            if (basicAttackColliderGO != null)
                basicAttackColliderGO.SetActive(false);
        }

        /// <summary>
        /// Enables attack lunging.
        /// </summary>
        protected void EnableAttackLunging()
        {
            _isAttackLunging = true;
        }

        /// <summary>
        /// Disables attack lunging.
        /// </summary>
        protected void DisableAttackLunging()
        {
            _isAttackLunging = false;
        }

        /// <summary>
        /// Fires a projectile and disables the projectile held by the mob (ONLY if mob is holding a proj).
        /// </summary>
        public virtual void FireProjectile()
        {
            // Archers may hold an arrow which needs to be set to off/on when firing
            if (projData.projHeld != null)
                projData.projHeld.SetActive(false);

            Vector3 playerPos = (player.position - transform.position).normalized;
            Vector3 targetDir = new Vector3(transform.forward.x, playerPos.y, transform.forward.z);
            Quaternion lookRot = Quaternion.LookRotation(targetDir);

            // Instantiate and initialize projectile
            GameObject go = Instantiate(projData.projPf, projData.projOrigin.position, lookRot);
            Projectile proj = go.GetComponent<Projectile>();
            Debug.Log("Proj GO: " + go);
            Debug.Log("Proj: " + proj);
            proj.Init(this, targetDir, projData.scalingStatData);
        }

        /// <summary>
        /// Enables the projectile held by the mob.
        /// </summary>
        public void EnableHeldProjectile()
        {
            if (projData.projHeld == null)
            {
                Debug.LogError("There is no reference to the projHeld variable.");
                return;
            }

            projData.projHeld.SetActive(true);
        }
        #endregion
    }
}
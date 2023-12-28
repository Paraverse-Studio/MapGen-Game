using Paraverse.Combat;
using Paraverse.Helper;
using Paraverse.Mob.Stats;
using System.Collections.Generic;
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

        [Header("Projectile Values")]
        [SerializeField, Tooltip("Set as true if mob is a projectile user.")]
        protected bool projUser = false;
        //[SerializeField]
        //protected ProjectileData projData;
        [SerializeField]
        protected BasicAttackSkill basicAttackSkill;
        public BasicAttackSkill BasicAttackSkill { get { return basicAttackSkill; } }

        // Constantly updates the distance from player
        protected float distanceFromTarget;
        public Transform Target { get { return _target; } set { _target = value; } }
        private Transform _target;

        public float BasicAtkRange { get { return basicAttackSkill.MaxRange; } }
        public bool IsBasicAttacking { get { return _isBasicAttacking; } }
        protected bool _isBasicAttacking = false;
        // Returns true when character is within basic attack range and cooldown is 0.
        public bool CanBasicAtk { get { return distanceFromTarget <= basicAttackSkill.MaxRange && distanceFromTarget >= basicAttackSkill.MinRange && basicAttackSkill.CurCooldown <= 0; } }
        // Sets to true when character is doing an action (Attack, Stun).
        public bool IsAttackLunging { get { return _isAttackLunging; } }
        protected bool _isAttackLunging = false;
        public bool IsSkilling { get; set; }
        public bool IsInCombat { get { return IsSkilling || IsBasicAttacking; } }

        [SerializeField, Tooltip("Mob skills.")]
        protected List<MobSkill> skills = new List<MobSkill>();
        public List<MobSkill> Skills { get { return skills; } }
        [SerializeField, Tooltip("Mob effects.")]
        protected List<MobEffect> effects = new List<MobEffect>();
        public List<MobEffect> Effects { get { return effects; } }

        protected int usingSkillIdx;
        [SerializeField]
        protected string animBool = "isUsingSkill";


        #region Skill One Delegates and Events
        // SKILL ONE
        // Enable/Disables main hand collider for Skill One
        public delegate void OnEnableMainHandColliderSOneDel();
        public event OnEnableMainHandColliderSOneDel OnEnableMainHandColliderSOneEvent;
        public delegate void OnDisableMainHandColliderSOneDel();
        public event OnEnableMainHandColliderSOneDel OnDisableMainHandColliderSOneEvent;
        // Enable/Disables off hand collider for Skill One
        public delegate void OnEnableOffHandColliderSOneDel();
        public event OnEnableOffHandColliderSOneDel OnEnableOffHandColliderSOneEvent;
        public delegate void OnDisableOffHandColliderSOneDel();
        public event OnDisableOffHandColliderSOneDel OnDisableOffHandColliderSOneEvent;
        // Enables/Disables special skill collider for Skill One
        public delegate void OnEnableSkillColliderSOneDel();
        public event OnEnableSkillColliderSOneDel OnEnableSkillColliderSOneEvent;
        public delegate void OnDisableSkillColliderSOneDel();
        public event OnDisableSkillColliderSOneDel OnDisableSkillColliderSOneEvent;
        // Handler special charging/releasing skills for Skill One
        public delegate void OnChargeSkillOneDel();
        public event OnChargeSkillOneDel OnChargeSkillOneEvent;
        public delegate void OnChargeCancelSkillOneDel();
        public event OnChargeCancelSkillOneDel OnChargeCancelSkillOneEvent;
        public delegate void OnEnableChargeReleaseSkillOneDel();
        public event OnEnableChargeReleaseSkillOneDel OnEnableChargeReleaseSkillOneEvent;
        // Used to disable anything at the end 
        public delegate void OnDisableSkillOneDel();
        public event OnDisableSkillOneDel OnDisableSkillOneEvent;
        // Used to summon 
        public delegate void OnSummonSkillOneDel();
        public event OnSummonSkillOneDel OnSummonSkillOneEvent;
        // Used to instantiate FXs
        public delegate void OnInstantiateFXOneDel();
        public event OnInstantiateFXOneDel OnInstantiateFXOneEvent;
        public delegate void OnSummonSkillTwoDel();
        public event OnSummonSkillTwoDel OnInstantiateFXTwoEvent;

        // Used for reaction to getting attack interrupted
        public delegate void OnAttackInterruptDel();
        public event OnAttackInterruptDel OnAttackInterrupted;

        // Event for existing enabling/disabling basic attack collider
        public delegate void OnEnableBasicAttackColliderDel();
        public event OnEnableBasicAttackColliderDel OnEnableBasicAttackCollider;
        public delegate void OnDisableBasicAttackColliderDel();
        public event OnDisableBasicAttackColliderDel OnDisableBasicAttackCollider;
        #endregion
        #endregion


        #region Start & Update Methods
        protected virtual void Start()
        {
            if (anim == null) anim = GetComponent<Animator>();
            if (player == null) player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
            if (player != null) _target = player;
            if (stats == null) stats = GetComponent<MobStats>();
            if (controller == null) controller = GetComponent<IMobController>();
            if (basicAttackSkill == null) Debug.LogError(transform.name + " has no basic attack skill! Please add a basic attack skill...");

            Initialize();
        }

        protected virtual void Update()
        {
            if (controller.IsDead) return;

            distanceFromTarget = ParaverseHelper.GetDistance(transform.position, player.position);
            _isBasicAttacking = anim.GetBool(StringData.IsBasicAttacking);

            if (anim.GetBool(StringData.IsUsingSkill))
                IsSkilling = true;
            else
                IsSkilling = false;

            // Gets active skill to run update method for each skill 
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].SkillUpdate();
                if (skills[i].skillOn)
                {
                    usingSkillIdx = i;
                }
            }
            basicAttackSkill.SkillUpdate();
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

            IsSkilling = false;
            if (null != basicAttackSkill)
            {
                basicAttackSkill.ActivateSkill(this, anim, stats, player);
            }
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].ActivateSkill(this, anim, stats, player);
                if (skills[i].skillOn)
                {
                    IsSkilling = true;
                }
            }
        }
        #endregion

        #region Basic Attack Logic
        /// <summary>
        /// Returns the basic attack cooldown based on attack speed value in stats.
        /// </summary>
        /// <returns></returns>
        protected float GetBasicAttackCooldown()
        {
            return 1f / stats.AttackSpeed.FinalValue;
        }
        #endregion

        /// <summary>
        /// Resets booelans when mob is interrupted during attack. 
        /// </summary>
        public virtual void OnAttackInterrupt()
        {
            _isAttackLunging = false;
            DisableBasicAttackCollider();
            IsSkilling = false;
            OnAttackInterrupted?.Invoke();
        }

        #region Animation Event Methods
        /// <summary>
        /// Enables basic weapon collider.
        /// </summary>
        protected void EnableBasicAttackCollider()
        {
            if (null != basicAttackSkill.attackColliderGO)
                basicAttackSkill.attackColliderGO.SetActive(true);

            OnEnableBasicAttackCollider?.Invoke();
        }

        /// <summary>
        /// Disables basic attack collider.
        /// </summary>
        protected void DisableBasicAttackCollider()
        {
            if (null != basicAttackSkill.attackColliderGO)
                basicAttackSkill.attackColliderGO.SetActive(false);

            OnDisableBasicAttackCollider?.Invoke();
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
            MobSkill skill;
            if (IsSkilling)
            {
                skill = skills[usingSkillIdx];
            }
            else
                skill = basicAttackSkill;

            // Archers may hold an arrow which needs to be set to off/on when firing
            if (skill.projData.projHeld != null)
                skill.projData.projHeld.SetActive(false);

            Vector3 playerPos = new Vector3(player.position.x, player.position.y + 0.5f, player.position.z);
            Vector3 targetDir = (playerPos - transform.position).normalized;

            Quaternion lookRot;
            if (skill.projData.projRotation == null)
                lookRot = Quaternion.LookRotation(targetDir);
            else
                lookRot = skill.projData.projRotation.rotation;

            // Instantiate and initialize projectile
            GameObject go = Instantiate(skill.projData.projPf, skill.projData.projOrigin.position, lookRot);
            Projectile proj = go.GetComponent<Projectile>();
            proj.Init(this, targetDir, skill.scalingStatData);
        }

        /// <summary>
        /// Enables the projectile held by the mob.
        /// </summary>
        public void EnableHeldProjectile()
        {
            MobSkill skill;
            if (IsSkilling)
                skill = skills[usingSkillIdx];
            else
                skill = basicAttackSkill;

            if (skill.projData.projHeld == null)
            {
                Debug.LogError("There is no reference to the projHeld variable for skill: ." + skill.name);
                return;
            }

            skill.projData.projHeld.SetActive(true);
        }
        #endregion

        public virtual void AEventInstantiateFXOne()
        {
            OnInstantiateFXOneEvent?.Invoke();
        }

        public virtual void AEventInstantiateFXTwo()
        {
            OnInstantiateFXTwoEvent?.Invoke();
        }

        public virtual void AEventEnableMainHandCollider()
        {
            OnEnableMainHandColliderSOneEvent?.Invoke();
        }

        public virtual void AEventDisableMainHandCollider()
        {
            OnDisableMainHandColliderSOneEvent?.Invoke();
        }

        public virtual void AEventEnableOffHandCollider()
        {
            OnEnableOffHandColliderSOneEvent?.Invoke();
        }

        public virtual void AEventDisableOffHandCollider()
        {
            OnDisableOffHandColliderSOneEvent?.Invoke();
        }

        public virtual void AEventEnableSkillCollider()
        {
            OnEnableSkillColliderSOneEvent?.Invoke();
        }

        public virtual void AEventDisableSkillCollider()
        {
            OnDisableSkillColliderSOneEvent?.Invoke();
        }

        public virtual void AEventChargeSkill()
        {
            OnChargeSkillOneEvent?.Invoke();
        }

        public virtual void AEventChargeCancelSkill()
        {
            OnChargeCancelSkillOneEvent?.Invoke();
        }

        public virtual void AEventChargeReleaseSkill()
        {
            OnEnableChargeReleaseSkillOneEvent?.Invoke();
        }

        public virtual void AEventDisableSkill()
        {
            OnDisableSkillOneEvent?.Invoke();
        }

        public virtual void AEventSummonSkill()
        {
            OnSummonSkillOneEvent?.Invoke();
        }
    }
}
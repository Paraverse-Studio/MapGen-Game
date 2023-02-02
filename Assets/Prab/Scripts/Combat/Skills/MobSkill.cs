using Paraverse.Helper;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

namespace Paraverse.Combat
{
    public class MobSkill : MonoBehaviour
    {
        #region Variables
        protected EnhancedMobCombat mob;
        protected Transform target;
        protected PlayerInputControls input;
        protected Animator anim;
        protected MobStats stats;

        public string Name { get { return _skillName; } set { _skillName = value; } }
        [SerializeField, Tooltip("Skill name.")]
        protected string _skillName = "";

        public int ID { get { return _ID; } set { _ID = value; } }
        [SerializeField, Tooltip("Skill ID.")]
        protected int _ID = -1;

        public Sprite Image { get { return _image; } set { _image = value; } }
        protected Sprite _image = null;

        public string Description { get { return _description; } set { _description = value; } }
        [SerializeField, TextArea(2, 3), Tooltip("Skill description.")]
        protected string _description = "";

        public bool TargetWithinRange { get { return IsInRange(); } }
        [SerializeField, Tooltip("Min skill range value.")]
        protected float _minRange = 0f;
        [SerializeField, Tooltip("Max skill range value.")]
        protected float _maxRange = 5f;
        public bool IsOffCooldown { get { return curCooldown <= 0; } }
        [SerializeField, Tooltip("Skill cooldown value.")]
        protected float cooldown = 5f;
        protected float curCooldown;

        public bool HasEnergy { get { return cost <= stats.CurEnergy; } }
        [SerializeField, Tooltip("Required energy cost to execute skill.")]
        protected float cost = 10f;

        [Tooltip("Name of skill animation to play.")]
        public string animName = "";

        [SerializeField]
        protected GameObject attackColliderGO;
        protected AttackCollider attackCollider;

        [Header("Projectile Values")]
        public ProjectileData projData;

        [Header("Uses Target Lock"), Tooltip("If this skill should force mob to face its target")]
        public bool usesTargetLock;
        [SerializeField, Tooltip("Speed of rotation during skill.")] 
        protected float rotSpeed = 100f;

        public ScalingStatData scalingStatData;

        public bool skillOn { get; set; }
        #endregion


        #region Inheritable Methods
        public virtual void ActivateSkill(EnhancedMobCombat mob, PlayerInputControls input, Animator anim, MobStats stats, Transform target = null)
        {
            this.mob = mob;
            this.target = target;
            this.input = input;
            this.anim = anim;
            this.stats = stats;
            curCooldown = 0f;
            if (mob.tag.Equals(StringData.PlayerTag))
            {
                input.OnSkillOneEvent += Execute;

                attackColliderGO = mob.basicAttackCollider.gameObject;
                attackCollider = attackColliderGO.GetComponent<AttackCollider>();
                if (null == projData.projOrigin) projData.projOrigin = attackColliderGO.transform;
            }
        }

        public virtual void ActivateSkill(EnhancedMobCombat mob, Animator anim, MobStats stats, Transform target = null)
        {
            this.mob = mob;
            this.target = target;
            this.anim = anim;
            this.stats = stats;
            curCooldown = 0f;
            if (mob.gameObject.CompareTag(StringData.PlayerTag))
                input.OnSkillOneEvent += Execute;

            if (null == attackColliderGO)
            {
                Debug.LogWarning(gameObject.name + " doesn't have an attack collider.");
                return;
            }
            attackColliderGO.SetActive(true);
            attackCollider = attackColliderGO.GetComponent<AttackCollider>();
            attackCollider.Init(mob, stats, scalingStatData, true);
            attackColliderGO.SetActive(false);
        }

        public virtual void DeactivateSkill(PlayerInputControls input)
        {
            if (mob.tag.Equals(StringData.PlayerTag))
                input.OnSkillOneEvent -= Execute;
        }

        /// <summary>
        /// Registers the skills animation events to the animation event methods in combat script.
        /// </summary>
        public virtual void SubscribeAnimationEventListeners()
        {

        }

        /// <summary>
        /// Unsubscribes the skills animation events to the animation event methods in combat script.
        /// </summary>
        public virtual void UnsubscribeAnimationEventListeners()
        {

        }

        /// <summary>
        /// Contains all methods required to run in Update within MobCombat script.
        /// </summary>
        public virtual void SkillUpdate()
        {
            if (null != target && mob.IsBasicAttacking == false && mob.IsSkilling == false)
            {
                Execute();
            }

            RotateToTarget();
            CooldownHandler();
        }

        protected virtual void RotateToTarget()
        {
            if (skillOn == false) return;

            if (usesTargetLock && input && mob.Target)
            {
                Vector3 targetDir = ParaverseHelper.GetPositionXZ(mob.Target.position - mob.transform.position).normalized;
                mob.transform.forward = targetDir;
            }
            else if (usesTargetLock && mob.Target)
            {
                //mob.transform.rotation = ParaverseHelper.FaceTarget(mob.transform, target.transform, 100f);
                Vector3 lookDir = (target.transform.position - mob.transform.position).normalized;
                Quaternion lookRot = Quaternion.LookRotation(lookDir);
                mob.transform.rotation = Quaternion.Slerp(mob.transform.rotation, lookRot, rotSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Run this method everytime a skill is activated
        /// </summary>
        protected virtual void ExecuteSkillLogic()
        {
            mob.IsSkilling = true;
            skillOn = true;
            anim.SetBool(StringData.IsUsingSkill, true);
            curCooldown = cooldown;
            stats.UpdateCurrentEnergy(-cost);
            anim.Play(animName);
        }

        protected virtual void DisableSkill()
        {
            skillOn = false;
            anim.SetBool(StringData.IsUsingSkill, false);

            UnsubscribeAnimationEventListeners();
        }

        /// <summary>
        /// Handles skill cooldown.
        /// </summary>
        protected virtual void CooldownHandler()
        {
            if (curCooldown > 0)
            {
                curCooldown -= Time.deltaTime;
            }
            curCooldown = Mathf.Clamp(curCooldown, 0f, cooldown);
        }

        /// <summary>
        /// Returns true if skill conditions are met. 
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanUseSkill()
        {
            if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false)
                return true;
            
            return false;
        }

        protected virtual bool IsInRange()
        {
            if (target == null) return true;

            float disFromTarget = ParaverseHelper.GetDistance(mob.transform.position, target.position);

            return disFromTarget >= _minRange && disFromTarget <= _maxRange;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Responsible for executing skill on button press.
        /// </summary>
        protected void Execute()
        {
            if (CanUseSkill())
            {
                SubscribeAnimationEventListeners();
                ExecuteSkillLogic();
            }
        }
        #endregion
    }
}



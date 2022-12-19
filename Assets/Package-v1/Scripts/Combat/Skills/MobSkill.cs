using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

namespace Paraverse.Combat
{
    public class MobSkill : MonoBehaviour, IMobSkill
    {
        #region Variables
        protected MobCombat mob;
        protected Transform target;
        protected PlayerInputControls input;
        protected Animator anim;
        protected IMobStats stats;

        public string Name { get { return _skillName; } }
        [SerializeField, Tooltip("Skill name.")]
        protected string _skillName = "";

        public string Description { get { return _description; } }
        [SerializeField, TextArea(2, 3), Tooltip("Skill description.")]
        protected string _description = "";

        public bool TargetWithinRange { get { return IsInRange(); } }
        [SerializeField, Tooltip("Skill range value.")]
        protected float _range = 5f;
        public bool IsOffCooldown { get { return curCooldown <= 0; } }
        [SerializeField, Tooltip("Skill cooldown value.")]
        protected float cooldown = 5f;
        protected float curCooldown;

        public bool HasEnergy { get { return cost <= stats.CurrentEnergy; } }
        [SerializeField, Tooltip("Required energy cost to execute skill.")]
        protected float cost = 10f;

        [SerializeField, Tooltip("Name of skill animation to play.")]
        protected string animName = "";

        [SerializeField]
        protected GameObject attackColliderGO;
        protected AttackCollider attackCollider;

        [Header("Projectile Values")]
        public ProjectileData projData;

        [Header("Damage Values")]
        [SerializeField, Range(0, 10)]
        protected float damageRatio = 1;

        public bool skillOn { get { return _skillOn; } }
        protected bool _skillOn = false;
        #endregion

        #region Public Methods
        public virtual void ActivateSkill(MobCombat mob, PlayerInputControls input, Animator anim, IMobStats stats, Transform target = null)
        {
            this.mob = mob;
            this.target = target;
            this.input = input;
            this.anim = anim;
            this.stats = stats;
            input.OnSkillOneEvent += Execute;
            curCooldown = 0f;

            if (null == attackCollider && null != attackColliderGO)
            {
                attackCollider = attackColliderGO.GetComponent<AttackCollider>();
                attackCollider.Init(mob, stats);
            }
        }

        public virtual void DeactivateSkill(PlayerInputControls input)
        {
            input.OnSkillOneEvent -= Execute;
        }

        /// <summary>
        /// Contains all methods required to run in Update within MobCombat script.
        /// </summary>
        public virtual void SkillUpdate()
        {
            CooldownHandler();
        }

        /// <summary>
        /// Responsible for executing skill on button press.
        /// </summary>
        public virtual void Execute()
        {
            if (CanUseSkill())
            {
                _skillOn = true;
                curCooldown = cooldown;
                stats.UpdateCurrentEnergy(-cost);
                anim.Play(animName);
                Debug.Log("Executing skill: " + _skillName + " which takes " + cost + " points of energy out of " + stats.CurrentEnergy + " point of current energy." +
                    "The max cooldown for this skill is " + cooldown + " and the animation name is " + animName + ".");
            }
        }
        #endregion

        #region Private Methods
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
            if (IsOffCooldown && HasEnergy && TargetWithinRange)
            {
                return true;
            }

            Debug.Log(_skillName + " is on cooldown or don't have enough energy!");
            return false;
        }

        protected bool IsInRange()
        {
            if (target == null) return true;

            float disFromTarget = ParaverseHelper.GetDistance(mob.transform.position, target.position);

            return disFromTarget <= _range;
        }
        #endregion
    }
}



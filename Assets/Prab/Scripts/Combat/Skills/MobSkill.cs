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

        public string Name { get { return _skillName; } set { _skillName = value; } }
        [SerializeField, Tooltip("Skill name.")]
        protected string _skillName = "";

        public string Description { get { return _description; } set { _skillName = value; } }
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

        [SerializeField, Tooltip("Name of skill animation to play.")]
        protected string animName = "";

        [SerializeField]
        protected GameObject attackColliderGO;
        protected AttackCollider attackCollider;

        [Header("Projectile Values")]
        public ProjectileData projData;

        [Header("Damage & Potency")]
        [SerializeField]
        public float flatPower = 1;

        [SerializeField, Range(0, 1)]
        public float attackScaling = 0;

        [SerializeField, Range(0, 1)]
        public float abilityScaling = 0;

        public bool skillOn { get; set; }
        #endregion

        #region Public Methods
        public virtual void ActivateSkill(MobCombat mob, PlayerInputControls input, Animator anim, IMobStats stats, Transform target = null)
        {
            this.mob = mob;
            this.target = target;
            this.input = input;
            this.anim = anim;
            this.stats = stats;
            curCooldown = 0f;
            if (mob.tag.Equals(StringData.PlayerTag))
                input.OnSkillOneEvent += Execute;

            if (null == attackCollider && null != attackColliderGO)
            {
                attackCollider = attackColliderGO.GetComponent<AttackCollider>();
                attackCollider.Init(mob, stats);
            }
        }

        public virtual void ActivateSkill(MobCombat mob, Animator anim, IMobStats stats, Transform target = null)
        {
            this.mob = mob;
            this.target = target;
            this.anim = anim;
            this.stats = stats;
            curCooldown = 0f;
            if (mob.tag.Equals(StringData.PlayerTag))
                input.OnSkillOneEvent += Execute;

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
            if (null != target && mob.IsBasicAttacking == false && mob.IsSkilling == false)
            {
                Execute();
            }
            CooldownHandler();
        }

        /// <summary>
        /// Responsible for executing skill on button press.
        /// </summary>
        public virtual void Execute()
        {
            if (CanUseSkill())
            {
                mob.IsSkilling = true;
                MarkSkillAsEnabled();
                curCooldown = cooldown;
                stats.UpdateCurrentEnergy(-cost);
                anim.Play(animName);
                Debug.Log("Executing skill: " + _skillName + " which takes " + cost + " points of energy out of " + stats.CurEnergy + " point of current energy." +
                    "The max cooldown for this skill is " + cooldown + " and the animation name is " + animName + ".");

                // depending on the skill, 
                // if it's a projectile, set its damage to:
                // int damage = (flatPower) + (mobStats.AttackDamage.FinalValue * attackScaling) + (mobStats.AbilityPower.FinalValue * abilityScaling);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Run this method everytime a skill is activated
        /// </summary>
        protected void MarkSkillAsEnabled()
        {
            skillOn = true;
            anim.SetBool(StringData.IsUsingSkill, true);
        }

        protected virtual void DisableSkill()
        {
            skillOn = false;
            anim.SetBool(StringData.IsUsingSkill, false);
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
            if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsBasicAttacking == false)
            {
                return true;
            }

            //Debug.Log(_skillName + " is on cooldown or don't have enough energy!");
            return false;
        }

        protected virtual bool IsInRange()
        {
            if (target == null) return true;

            float disFromTarget = ParaverseHelper.GetDistance(mob.transform.position, target.position);

            return disFromTarget >= _minRange && disFromTarget <= _maxRange;
        }
        #endregion
    }
}



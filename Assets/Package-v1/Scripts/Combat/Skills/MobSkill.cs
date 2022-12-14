using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

namespace Paraverse.Combat
{
    public class MobSkill : MonoBehaviour
    {
        #region Variables
        protected PlayerInputControls input;
        protected Animator anim;
        protected IMobStats stats;

        public string Name { get { return _skillName; } }
        [SerializeField, Tooltip("Skill name.")]
        protected string _skillName = "";

        public string Description { get { return _description; } }
        [SerializeField, TextArea(2, 3), Tooltip("Skill description.")]
        protected string _description = "";

        public bool IsOffCooldown { get { return curCooldown <= 0; } }
        [SerializeField, Tooltip("Skill cooldown value.")]
        protected float cooldown = 5f;
        protected float curCooldown;

        public bool HasEnergy { get { return cost <= stats.CurrentEnergy; } }
        [SerializeField, Tooltip("Required energy cost to execute skill.")]
        protected float cost = 10f;

        [SerializeField, Tooltip("Name of skill animation to play.")]
        protected string animName = "";

        private delegate void method();
        #endregion

        #region Start Method
        public virtual void ActivateSkill(PlayerInputControls input, Animator anim, IMobStats stats)
        {
            this.input = input;
            this.anim = anim;
            this.stats = stats;
            input.OnSkillOneEvent += Execute;
            curCooldown = 0f;
        }

        public virtual void DeactivateSkill(PlayerInputControls input)
        {
            Debug.Log("input: " + input);
            input.OnSkillOneEvent -= Execute;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Contains all methods required to run in Update within MobCombat script.
        /// </summary>
        public virtual void SkillUpdate()
        {
            CooldownHandler();
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
        #endregion

        #region Skill Execution Conditions
        /// <summary>
        /// Responsible for executing skill on button press.
        /// </summary>
        public virtual void Execute()
        {
            if (CanUseSkill())
            {
                curCooldown = cooldown;
                stats.UpdateCurrentEnergy(-cost);
                anim.Play(animName);
                Debug.Log("Executing skill: " + _skillName + " which takes " + cost + " points of energy out of " + stats.CurrentEnergy + " point of current energy." +
                    "The max cooldown for this skill is " + cooldown + " and the animation name is " + animName + ".");
            }
        }

        /// <summary>
        /// Returns true if skill conditions are met. 
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanUseSkill()
        {
            if (IsOffCooldown && HasEnergy)
            {
                return true;
            }

            Debug.Log(_skillName + " is on cooldown or don't have enough energy!");
            return false;
        }
        #endregion
    }
}



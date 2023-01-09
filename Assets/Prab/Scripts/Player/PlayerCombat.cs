using Paraverse.Combat;
using Paraverse.Mob.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace Paraverse.Player
{
    public class PlayerCombat : MobCombat
    {
        #region Variables
        private new PlayerController controller;
        private PlayerInputControls input;

        // Basic attack combo variables
        public int BasicAttackComboIdx { get { return basicAtkComboIdx; } }
        private int basicAtkComboIdx = 0;
        private int basicAtkComboIdxLimit = 2;
        [SerializeField, Tooltip("Max cooldown to allow next combo attack.")]
        private float maxComboResetTimer = 1f;
        private float curCombatResetTimer;

        private int usingSkillIdx;

        public bool CanComboAttackTwo { get { return _canComboAttackTwo; } }
        private bool _canComboAttackTwo = false;
        public bool CanComboAttackThree { get { return _canComboAttackThree; } }
        private bool _canComboAttackThree = false;

        // Skills 
        public List<MobSkill> skills = new List<MobSkill>();

        private MobSkill _activeSkill;
        public MobSkill ActiveSkill { get { return _activeSkill; } }
        #endregion

        #region Start & Update Methods
        protected override void Start()
        {
            base.Start();
            controller = gameObject.GetComponent<PlayerController>();
            input = GetComponent<PlayerInputControls>();
            input.OnBasicAttackEvent += ApplyBasicAttack;

            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].ActivateSkill(this, input, anim, stats);
            }
        }

        public void ActivateSkill(MobSkill skill)
        {


            if (skills.Contains(skill))
            {
                skill.ActivateSkill(this, input, anim, stats);
                _activeSkill = skill;
            }


        }

        protected override void Update()
        {
            base.Update();
            AnimationHandler();
            BasicAttackComboHandler();
            
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].SkillUpdate();
                if (skills[i].skillOn)
                {
                    usingSkillIdx = i;
                }
            }
        }
        #endregion

        #region Animation Handler
        private void AnimationHandler()
        {
            _canComboAttackTwo = anim.GetBool(StringData.CanBasicAttackTwo);
            _canComboAttackThree = anim.GetBool(StringData.CanBasicAttackThree);
        }
        #endregion 

        #region Basic Attack Methods
        /// <summary>
        /// Runs on OnBasicAttackEvent
        /// </summary>
        private void ApplyBasicAttack()
        {
            if (controller.IsAvoidingObjUponLanding) return;

            if (controller.IsInteracting == false || anim.GetBool(StringData.CanBasicAttackTwo) || anim.GetBool(StringData.CanBasicAttackThree))
            {
                PlayBasicAttackCombo();
            }
        }

        /// <summary>
        /// Plays the basic attack animation based on the basic attack combo.
        /// </summary>
        private void PlayBasicAttackCombo()
        {
            if (anim.GetBool(StringData.CanBasicAttackThree))
            {
                anim.SetBool(StringData.IsInteracting, true);
                anim.Play(StringData.BasicAttackThree);
            }
            else if (anim.GetBool(StringData.CanBasicAttackTwo))
            {
                anim.SetBool(StringData.IsInteracting, true);
                anim.Play(StringData.BasicAttackTwo);
            }
            else if (basicAtkComboIdx == 0)
            {
                anim.Play(StringData.BasicAttack);
            }

            // Increment combo index upon basic attack
            ResetAnimationComboStates();
            basicAtkComboIdx++;
            curCombatResetTimer = maxComboResetTimer;
            if (basicAtkComboIdx > basicAtkComboIdxLimit)
            {
                basicAtkComboIdx = 0;
            }
        }

        /// <summary>
        /// Resets the animation event booleans.
        /// </summary>
        private void ResetAnimationComboStates()
        {
            _isAttackLunging = false;
            anim.SetBool(StringData.CanBasicAttackTwo, false);
            anim.SetBool(StringData.CanBasicAttackThree, false);
        }

        /// <summary>
        /// Resets basic attack combo index to 0 when curCombatResetTimer reaches 0.
        /// </summary>
        private void BasicAttackComboHandler()
        {
            if (curCombatResetTimer <= 0)
            {
                ResetAnimationComboStates();
                basicAtkComboIdx = 0;
            }
            else
            {
                curCombatResetTimer -= Time.deltaTime;
            }
        }
        #endregion

        #region Skill Methods
        /// <summary>
        /// Adds skill to mobs active skills.
        /// </summary>
        /// <param name="skill"></param>
        public void AddToActiveSkills(MobSkill skill)
        {
            if (skills.Contains(skill))
            {
                Debug.Log(skill.Name + " already exists in the Active Skills OR you have max number of active skills.");
                return;
            }
            if (skills.Count > 0)
                RemoveFromActiveSkills(skills[skills.Count-1]);
            
            skills.Add(skill);
            skill.ActivateSkill(this, input, anim, stats);
        }

        /// <summary>
        /// Removes skill from mob active skills and removes listener.
        /// </summary>
        /// <param name="skill"></param>
        public void RemoveFromActiveSkills(MobSkill skill)
        {
            Debug.Log("skill: " + skill.Name);
            if (skills.Count <= 0)
            {
                Debug.Log("No skill exists in Active Skills.");
                return;
            }
            skill.DeactivateSkill(input);
            skills.Remove(skill);
            Debug.Log("Remove skill: " + skill.Name + " from Active Skills: " + skills.Count);
        }
        #endregion

        #region Animation Events
        public void AllowComboAttackTwo()
        {
            ResetAnimationComboStates();
            anim.SetBool(StringData.CanBasicAttackTwo, true);
        }

        public void AllowComboAttackThree()
        {
            ResetAnimationComboStates();
            anim.SetBool(StringData.CanBasicAttackThree, true);
        }

        public override void FireProjectile()
        {
            ProjectileData data;

            if (anim.GetBool(StringData.IsUsingSkill))
                data = skills[usingSkillIdx].projData;
            else
                data = projData;

            // Archers may hold an arrow which needs to be set to off/on when firing
            if (data.projHeld != null)
                data.projHeld.SetActive(false);

            //// Instantiate and initialize projectile
            GameObject go = Instantiate(data.projPf, data.projOrigin.position, transform.rotation);
            Projectile proj = go.GetComponent<Projectile>();
            proj.Init(this, transform.forward, basicAtkDmgRatio * stats.AttackDamage.FinalValue);

            skills[usingSkillIdx].skillOn = false;
        }
        #endregion
    }
}

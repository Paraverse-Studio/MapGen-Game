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

        public bool CanComboAttackTwo { get { return _canComboAttackTwo; } }
        private bool _canComboAttackTwo = false;
        public bool CanComboAttackThree { get { return _canComboAttackThree; } }
        private bool _canComboAttackThree = false;


        // Skills 
        [SerializeField]
        private List<MobSkill> ActiveSkills = new List<MobSkill>();
        [SerializeField]
        private List<MobSkill> SkillsContainer = new List<MobSkill>();
        [SerializeField, Tooltip("Max allowed active skills.")]
        private int maxActiveSkills = 1;

        [SerializeField]
        private List<MobSkill> SKILLS = new List<MobSkill>();
        #endregion

        #region Start & Update Methods
        protected override void Start()
        {
            base.Start();
            controller = gameObject.GetComponent<PlayerController>();
            input = GetComponent<PlayerInputControls>();
            input.OnBasicAttackEvent += ApplyBasicAttack;

            for (int i = 0; i < ActiveSkills.Count; i++)
            {
                ActiveSkills[i].ActivateSkill(transform, input, anim, stats);
            }
        }

        private int counter = 0;
        private int activeCounter = 0;
        protected override void Update()
        {
            base.Update();
            AnimationHandler();
            BasicAttackComboHandler();

            for (int i = 0; i < ActiveSkills.Count; i++)
            {
                ActiveSkills[i].SkillUpdate();
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                switch (counter)
                {
                    case 0:
                        AddToSkillsContainer(SKILLS[0]);
                        break;
                    case 1:
                        AddToSkillsContainer(SKILLS[1]);
                        break;
                    case 2:
                        AddToSkillsContainer(SKILLS[2]);
                        break;
                    case 3:
                        AddToSkillsContainer(SKILLS[3]);
                        break;
                }
                counter++;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                switch (activeCounter)
                {
                    case 0:
                        AddToActiveSkills(SkillsContainer[0]);
                        break;
                    case 1:
                        AddToActiveSkills(SkillsContainer[1]);
                        break;
                    case 2:
                        AddToActiveSkills(SkillsContainer[2]);
                        break;
                    case 3:
                        AddToActiveSkills(SkillsContainer[3]);
                        break;
                }
                activeCounter++;
                if (activeCounter > 3)
                    activeCounter = 0;
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
            if (ActiveSkills.Contains(skill))
            {
                Debug.Log(skill.Name + " already exists in the Active Skills OR you have max number of active skills.");
                return;
            }
            if (ActiveSkills.Count > 0)
                RemoveFromActiveSkills(ActiveSkills[ActiveSkills.Count-1]);
            
            ActiveSkills.Add(skill);
            skill.ActivateSkill(transform, input, anim, stats);
        }

        /// <summary>
        /// Removes skill from mob active skills and removes listener.
        /// </summary>
        /// <param name="skill"></param>
        public void RemoveFromActiveSkills(MobSkill skill)
        {
            Debug.Log("skill: " + skill.Name);
            if (ActiveSkills.Count <= 0)
            {
                Debug.Log("No skill exists in Active Skills.");
                return;
            }
            skill.DeactivateSkill(input);
            ActiveSkills.Remove(skill);
            Debug.Log("Remove skill: " + skill.Name + " from Active Skills: " + ActiveSkills.Count);
        }

        /// <summary>
        /// Adds skills to mobs skills container.
        /// </summary>
        /// <param name="skill"></param>
        public void AddToSkillsContainer(MobSkill skill)
        {
            if (SkillsContainer.Contains(skill))
            {
                Debug.Log(skill.Name + " already exists in the Skills Container.");
                return;
            }
            SkillsContainer.Add(skill);
        }

        /// <summary>
        /// Removes skill from mobs skills container. 
        /// </summary>
        /// <param name="skill"></param>
        public void RemoveFromSkillsContainer(MobSkill skill)
        {
            if (SkillsContainer.Count <= 0)
            {
                Debug.Log("No skill exists in Skills Container.");
                return;
            }
            SkillsContainer.Remove(skill);
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
        #endregion
    }
}

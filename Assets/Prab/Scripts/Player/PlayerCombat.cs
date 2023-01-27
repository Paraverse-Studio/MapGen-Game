using Paraverse.Combat;
using Paraverse.Helper;
using Paraverse.Mob.Stats;
using UnityEngine;

namespace Paraverse.Player
{
    public class PlayerCombat : EnhancedMobCombat
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
        [SerializeField]
        private Transform _skillHolder;
        public Transform SkillHolder => _skillHolder;
        public bool CanComboAttackTwo { get { return _canComboAttackTwo; } }
        private bool _canComboAttackTwo = false;
        public bool CanComboAttackThree { get { return _canComboAttackThree; } }
        private bool _canComboAttackThree = false;

        // Skills 
        private MobSkill _activeSkill;
        public MobSkill ActiveSkill { get { return _activeSkill; } }
        #endregion


        #region Start & Update Methods
        protected override void Start()
        {
            if (anim == null) anim = GetComponent<Animator>();
            if (player == null) player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
            if (stats == null) stats = GetComponent<IMobStats>();
            if (controller == null) controller = GetComponent<PlayerController>();

            Initialize();

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
            if (null == skill) return;
            if (null != _activeSkill) _activeSkill.DeactivateSkill(input);

            foreach (MobSkill sk in skills)
            {
                if (sk.ID == skill.ID)
                {
                    sk.ActivateSkill(this, input, anim, stats);
                    _activeSkill = sk;
                    return;
                }
            }
            MobSkill newSkill = Instantiate(skill, SkillHolder);
            skills.Add(newSkill);
            newSkill.ActivateSkill(this, input, anim, stats);
            _activeSkill = newSkill;
        }

        protected override void Update()
        {
            if (controller.IsDead) return;

            distanceFromTarget = ParaverseHelper.GetDistance(transform.position, player.position);

            _isBasicAttacking = anim.GetBool(StringData.IsBasicAttacking);
            BasicAttackComboHandler();
            AttackCooldownHandler();
            AnimationHandler();

            if (anim.GetBool(StringData.IsUsingSkill))
                IsSkilling = true;
            else
                IsSkilling = false;

            // Gets active skill to run update method for each skill 
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].SkillUpdate();
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

            if (controller.IsInteracting == false || anim.GetBool(StringData.CanBasicAttackTwo) || anim.GetBool(StringData.CanBasicAttackThree) ||
                controller.IsDiving)
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
                if (controller.IsDiving) anim.Play(StringData.BasicAttackTwo); // dash-attacking
                else anim.Play(StringData.BasicAttack);
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
                RemoveFromActiveSkills(skills[skills.Count - 1]);

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
            float damage = basicAtkDmgRatio * stats.AttackDamage.FinalValue;

            if (IsSkilling)
            {
                MobSkill s = _activeSkill;
                data = s.projData;
                damage = s.scalingStatData.flatPower + (stats.AttackDamage.FinalValue * s.scalingStatData.attackScaling) + (stats.AbilityPower.FinalValue * s.scalingStatData.abilityScaling);
            }
            else
                data = projData;

            // Archers may hold an arrow which needs to be set to off/on when firing
            if (data.projHeld != null)
                data.projHeld.SetActive(false);

            //// Instantiate and initialize projectile
            GameObject go = Instantiate(data.projPf, data.projOrigin.position, transform.rotation);
            Projectile proj = go.GetComponent<Projectile>();
            proj.Init(this, transform.forward, damage);

            _activeSkill.skillOn = false;
            IsSkilling = false;
        }
        #endregion
    }
}

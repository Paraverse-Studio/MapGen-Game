using Paraverse.Combat;
using Paraverse.Helper;
using Paraverse.Mob.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField]
        private Transform _effectsHolder;
        public Transform EffectsHolder => _effectsHolder;
        public bool CanComboAttackTwo { get { return _canComboAttackTwo; } }
        private bool _canComboAttackTwo = false;
        public bool CanComboAttackThree { get { return _canComboAttackThree; } }
        private bool _canComboAttackThree = false;

        public GameObject AttackColliderGO => _attackColliderGO;
        [SerializeField]
        private GameObject _attackColliderGO;

        // Skills 
        [SerializeField]
        private MobSkill _activeSkill;
        public MobSkill ActiveSkill { get { return _activeSkill; } }

        [Header("SKill U.I.")]
        [SerializeField] private TextMeshProUGUI _skillLabel;
        [SerializeField] private TextMeshProUGUI _skillCDTime;
        [SerializeField] private Image _skillCDFill;
        [SerializeField] private Image _skillIcon;
        [SerializeField] private Animation _skillCDGlow;
        [SerializeField] private ContentFitterRefresher _refresher;


        // Reset to Default Skill UI upon player death
        [SerializeField]
        private Sprite noSkillSprite;
        #endregion


        #region Start & Update Methods
        protected override void Start()
        {
            if (anim == null) anim = GetComponent<Animator>();
            if (player == null) player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
            if (stats == null) stats = GetComponent<MobStats>();
            if (controller == null) controller = GetComponent<PlayerController>();

            Initialize();

            controller = gameObject.GetComponent<PlayerController>();
            input = GetComponent<PlayerInputControls>();
            input.OnBasicAttackEvent += ApplyBasicAttack;

            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].ActivateSkill(this, input, anim, stats);
            }
            for (int i = 0; i < effects.Count; i++)
            {
                effects[i].ActivateEffect(stats);
            }
        }

        public void ActivateSkill(GameObject obj)
        {
            MobSkill skill = obj.GetComponent<MobSkill>();

            if (null != _activeSkill) _activeSkill.DeactivateSkill(input);

            foreach (MobSkill sk in skills)
            {
                if (sk.ID == skill.ID)
                {
                    ActivateSkillWithUI(sk);
                    return;
                }
            }
            MobSkill skillInstance = Instantiate(obj, SkillHolder).GetComponent<MobSkill>();
            skills.Add(skillInstance);
            ActivateSkillWithUI(skillInstance);
        }

        public void DeactivateSkill()
        {
            if (null != _activeSkill)
            {
                _activeSkill.DeactivateSkill(input);
            }

            for (int i = 0; i < skills.Count; i++)
            {
                DeactivateSkillWithUI(skills[i]);
                skills.Remove(skills[i]);
            }
        }

        private void ActivateSkillWithUI(MobSkill skill)
        {
            skill.ActivateSkill(this, input, anim, stats);
            _activeSkill = skill;
            _skillLabel.text = skill.Name;
            _skillLabel.transform.parent.gameObject.SetActive(true);
            _skillIcon.sprite = skill.Image;
            _refresher.RefreshContentFitters();

            // Apply Effects upon skill change
            foreach (MobEffect effect in Effects)
            {
                effect.OnSkillChangeApplyEffect();
            }
        }

        private void DeactivateSkillWithUI(MobSkill skill)
        {
            skill.DeactivateSkill(input);
            _activeSkill = null;
            _skillLabel.text = "";
            _skillCDTime.text = "";
            _skillLabel.transform.parent.gameObject.SetActive(false);
            _skillIcon.sprite = noSkillSprite;
            _skillCDFill.gameObject.SetActive(false);
            _refresher.RefreshContentFitters();
        }

        public void ActivateEffect(GameObject obj)
        {
            MobEffect effect = obj.GetComponent<MobEffect>();
            if (null == effect) return;

            foreach (MobEffect eff in effects)
            {
                if (eff.ID == effect.ID)
                {
                    eff.ActivateEffect(stats);
                    return;
                }
            }
            MobEffect effectObj = Instantiate(obj, EffectsHolder).GetComponent<MobEffect>();
            effects.Add(effectObj);
            effectObj.ActivateEffect(stats);
        }

        public void DeactivateEffects()
        {
            foreach (MobEffect eff in effects)
            {
                eff.DeactivateEffect();
                Destroy(eff.gameObject);
            }
            effects.Clear();
        }

        protected override void Update()
        {
            //TEST_METHOD();
            if (controller.IsDead) return;

            distanceFromTarget = ParaverseHelper.GetDistance(transform.position, player.position);

            _isBasicAttacking = anim.GetBool(StringData.IsBasicAttacking);
            BasicAttackComboHandler();
            AnimationHandler();

            if (anim.GetBool(StringData.IsUsingSkill))
                IsSkilling = true;
            else
                IsSkilling = false;

            // Gets active skill to run update method for each skill 
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].SkillUpdate();
                SkillUIHandler();
            }
        }

        private void TEST_METHOD()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                DeactivateEffects();
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

        #region Skill Update UI Handler
        /// <summary>
        /// Updates the current active skill's UI components (visuals)
        /// </summary>
        private void SkillUIHandler()
        {
            if (_activeSkill)
            {
                if (_activeSkill.IsOffCooldown)
                {
                    if (_skillCDFill.gameObject.activeSelf) _skillCDGlow.Play();
                    _skillCDFill.gameObject.SetActive(false);
                }
                else
                {
                    _skillCDFill.gameObject.SetActive(true);
                    _skillCDFill.fillAmount = _activeSkill.CurCooldown / _activeSkill.Cooldown;
                    _skillCDTime.text = Mathf.CeilToInt(_activeSkill.CurCooldown).ToString();
                }
            }
        }
        #endregion

        #region Basic Attack Methods
        /// <summary>
        /// Runs on OnBasicAttackEvent
        /// </summary>
        private void ApplyBasicAttack()
        {
            if (controller.IsAvoidingObjUponLanding || IsSkilling) return;

            if (controller.IsInteracting == false || anim.GetBool(StringData.CanBasicAttackTwo) || anim.GetBool(StringData.CanBasicAttackThree) || controller.IsDiving)
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
            MobSkill skill = null;

            if (IsSkilling)
            {
                skill = _activeSkill;
            }
            else
            {
                skill = basicAttackSkill;
                Debug.LogError("Invoked PlayerCombat's FireProjectile without providing proper projectile data.");
            }

            // Archers may hold an arrow which needs to be set to off/on when firing
            if (basicAttackSkill.projData.projHeld != null)
                basicAttackSkill.projData.projHeld.SetActive(false);

            // Added to assist in auto aiming to enemy when targetted 
            Vector3 targetDir = transform.forward;
            if (Target) targetDir = ParaverseHelper.GetPositionXZ(Target.transform.position - transform.position).normalized;

            //// Instantiate and initialize projectile
            if (null != skill.projData.projPf)
            {
                GameObject go = Instantiate(skill.projData.projPf, transform.position, transform.rotation);
                Projectile proj = go.GetComponent<Projectile>();
                proj.Init(this, targetDir, skill.scalingStatData);

                // Adds effect listeners to newly instantiated projectiles (OnAttackApplyDamage, OnAttackPostDamage, etc)
                foreach (MobEffect effect in Effects)
                {
                    effect.AddSubscribersToSkillEvents(proj);
                }
            }
            else Debug.LogError("A skill invoked PlayerCombat's FireProjectile without providing proper projectile data, and no default data.");

            _activeSkill.skillOn = false;
            anim.SetBool(StringData.IsUsingSkill, false);
        }
        #endregion
    }
}

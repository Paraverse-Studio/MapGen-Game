using Paraverse.Combat;
using Paraverse.Helper;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Paraverse.Player
{
  public class PlayerCombat : MobCombat, IMobCombat
  {
    #region Variables
    private new PlayerController controller;
    private PlayerInputControls input;

    // Basic attack combo variables
    public int BasicAttackComboIdx { get => basicAtkComboIdx; }
    private int basicAtkComboIdx = 0;
    private int basicAtkComboIdxLimit = 2;
    [SerializeField, Tooltip("Max cooldown to allow next combo attack.")]
    private float maxComboResetTimer = 1f;
    private float curCombatResetTimer;
    [SerializeField]
    public Transform SkillHolder => _skillHolder;
    [SerializeField]
    private Transform _skillHolder;
    public Transform EffectsHolder => _effectsHolder;
    [SerializeField]
    private Transform _effectsHolder;
    public bool CanComboAttackTwo { get => _canComboAttackTwo; }
    private bool _canComboAttackTwo = false;
    public bool CanComboAttackThree { get => _canComboAttackThree; }
    private bool _canComboAttackThree = false;

    public GameObject AttackColliderGO => _attackColliderGO;
    [SerializeField]
    private GameObject _attackColliderGO;

    [Header("SKill U.I.")]
    [SerializeField]
    private TextMeshProUGUI _skillLabel;
    [SerializeField]
    private TextMeshProUGUI _skillCDTime;
    [SerializeField]
    private Image _skillCDFill;
    [SerializeField]
    private Image _skillIcon;
    [SerializeField]
    private Animation _skillCDGlow;
    [SerializeField]
    private ContentFitterRefresher _refresher;

    // Reset to Default Skill UI upon player death
    [SerializeField]
    private Sprite noSkillSprite;
    #endregion

    #region Initializing Methods
    protected override void Start()
    {
      if (anim == null) anim = GetComponent<Animator>();
      if (player == null) player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
      if (stats == null) stats = GetComponent<MobStats>();
      if (controller == null) controller = GetComponent<PlayerController>();
      input = GetComponent<PlayerInputControls>();
      input.OnBasicAttackEvent += ApplyBasicAttack;

      for (int i = 0; i < _skills.Count; i++)
      {
        _skills[i].ActivateSkill(this, input, anim, stats);
      }
      for (int i = 0; i < _effects.Count; i++)
      {
        _effects[i].ActivateEffect(stats, _effects[i].ID, _effects[i].effectLevel);
      }
      _attackColliderGO.GetComponent<AttackCollider>().Init(this);

      Initialize();
    }

    public void ActivateSkill(GameObject obj)
    {
      MobSkill skill = obj.GetComponent<MobSkill>();

      if (null != _activeSkill) _activeSkill.DeactivateSkill(input);

      foreach (MobSkill sk in _skills)
      {
        if (sk.ID == skill.ID)
        {
          ActivateSkillWithUI(sk);
          return;
        }
      }
      MobSkill skillInstance = Instantiate(obj, SkillHolder).GetComponent<MobSkill>();
      _skills.Add(skillInstance);
      ActivateSkillWithUI(skillInstance);
    }

    public void DeactivateSkill()
    {
      if (null != _activeSkill)
      {
        _activeSkill.DeactivateSkill(input);
      }

      foreach (MobSkill skill in _skills)
      {
        DeactivateSkillWithUI(skill);
        Destroy(skill.gameObject);
      }
      _skills.Clear();
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

    public void ActivateEffect(GameObject obj, int id, int level)
    {
      MobEffect effect = obj.GetComponent<MobEffect>();
      if (null == effect) return;

      // If effect already exists, then just activate
      foreach (MobEffect eff in _effects)
      {
        if (eff.ID == effect.ID)
        {
          eff.DeactivateEffect();
          _effects.Remove(eff);
          Destroy(eff.gameObject);
          break;
        }
      }

      // add effect to EffectsHolder if it doesn't exist
      MobEffect effectObj = Instantiate(obj, EffectsHolder).GetComponent<MobEffect>();
      // Initialize the effect objects values here !!!
      effectObj.ID = id;
      effectObj.effectLevel = level;
      _effects.Add(effectObj);
      effectObj.ActivateEffect(stats, id, level);
    }

    public void DeactivateEffects()
    {
      for (int i = 0; i < _effects.Count; i++)
      {
        _effects[i].DeactivateEffect();
        Destroy(_effects[i].gameObject);
      }
      _effects.Clear();
    }
    #endregion

    protected override void Update()
    {
      if (controller.IsDead) return;

      distanceFromTarget = ParaverseHelper.GetDistance(transform.position, player.position);

      IsBasicAttacking = anim.GetBool(StringData.IsBasicAttacking);
      BasicAttackComboHandler();
      AnimationHandler();

      if (anim.GetBool(StringData.IsUsingSkill))
        IsSkilling = true;
      else
        IsSkilling = false;

      if (IsSkilling || IsBasicAttacking)
        _isAttacking = true;
      else
        _isAttacking = false;

      // Gets active skill to run update method for each skill 
      for (int i = 0; i < _skills.Count; i++)
      {
        _skills[i].SkillUpdate();
        SkillUIHandler();
      }
    }

    #region Inherited Methods From MobCombat
    public override void OnAttackInterrupt()
    {
      base.OnAttackInterrupt();
      anim.SetBool(StringData.IsUsingSkill, false);
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
      if (controller.IsAvoidingObjUponLanding || IsSkilling || controller.IsDead) return;

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
      MobSkill skill;

      // Need to fix this for player as ActiveSkill is always active
      if (IsSkilling)
        skill = ActiveSkill;
      else
      {
        skill = _basicAttackSkill;
        Debug.LogError("Invoked PlayerCombat's FireProjectile without providing proper projectile data.");
      }

      // Archers may hold an arrow which needs to be set to off/on when firing
      if (skill.projData.projHeld != null)
        skill.projData.projHeld.SetActive(false);

      // Added to assist in auto aiming to enemy when targetted 
      Vector3 targetDir = transform.forward;
      if (Target) targetDir = ParaverseHelper.GetPositionXZ(Target.transform.position - transform.position).normalized;

      //// Instantiate and initialize projectile
      if (null != skill.projData.projPf)
      {
        GameObject go = Instantiate(skill.projData.projPf, transform.position, skill.transform.rotation);
        Projectile proj = go.GetComponent<Projectile>();
        proj.Init(this, targetDir, skill.projData.projSpeed, skill.scalingStatData);

        // Adds effect listeners to newly instantiated projectiles (OnAttackApplyDamage, OnAttackPostDamage, etc)
        foreach (MobEffect effect in Effects)
        {
          effect.AddSubscribersToSkillEvents(proj);
        }
      }
      else Debug.LogError("A skill invoked PlayerCombat's FireProjectile without providing proper projectile data, and no default data.");


      // Sets SkillStaet to InActive and reset skill cooldown
      AEventDisableSkill();
    }
    #endregion
  }
}

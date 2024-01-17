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
    public Animator Anim => anim;
    protected Animator anim;

    // Required reference scripts
    public MobStats Stats => stats;
    protected MobStats stats;
    public IMobController Controller => controller;
    protected IMobController controller;

    [Header("Target Values")]
    [Tooltip("Target tag (Player by default).")]
    protected string targetTag = StringData.PlayerTag;
    protected Transform player;
    public Transform Target { get => _target; set => _target = value; }
    private Transform _target;

    [SerializeField, Tooltip("Set as true if mob is a projectile user.")]
    protected bool projUser = false;
    // Constantly updates the distance from player
    protected float distanceFromTarget;

    // Basic Attacks
    public BasicAttackSkill BasicAttackSkill => _basicAttackSkill;
    [SerializeField]
    protected BasicAttackSkill _basicAttackSkill;
    public float BasicAtkRange => _basicAttackSkill.MaxRange;
    public bool IsAttacking => _isAttacking;
    protected bool _isAttacking = false;
    public bool IsBasicAttacking { get; set; }
    // Returns true when character is within basic attack range and cooldown is 0.
    public bool CanBasicAtk => distanceFromTarget <= _basicAttackSkill.MaxRange && distanceFromTarget >= _basicAttackSkill.MinRange && _basicAttackSkill.CurCooldown <= 0;
    // Sets to true when character is doing an action (Attack, Stun).
    public bool IsSkilling { get; set; }
    public bool IsAttackLunging => _isAttackLunging;
    protected bool _isAttackLunging = false;
    public bool IsInCombat => IsSkilling || IsBasicAttacking;
    protected int usingSkillIdx;

    public List<MobSkill> Skills => _skills;
    [SerializeField]
    protected List<MobSkill> _skills = new List<MobSkill>();
    public MobSkill ActiveSkill => _activeSkill;
    [SerializeField]
    protected MobSkill _activeSkill;
    public List<MobEffect> Effects => _effects;
    [SerializeField]
    protected List<MobEffect> _effects = new List<MobEffect>();

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


    // safety feature to disable skill if active too long
    [SerializeField]
    protected bool useSetSkillToCompleteSafetyFeature = true;
    [SerializeField]
    protected float setSkillToCompleteTimer = 10f;
    protected float curSetSkillToCompleteTimer = 0;
    #endregion
    #endregion

    protected virtual void Start()
    {
      if (anim == null) anim = GetComponent<Animator>();
      if (player == null) player = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
      if (player != null) _target = player;
      if (stats == null) stats = GetComponent<MobStats>();
      if (controller == null) controller = GetComponent<IMobController>();
      
      bool basicAttackExists = false;
      
      // Checks if BasicAttackSkill exists in Skills list, of not, add
      for (int i = 0; i < _skills.Count; i++)
      {
        if (_skills[i].IsBasicAttack)
        {
          basicAttackExists = true;
          _basicAttackSkill = _skills[i].GetComponent<BasicAttackSkill>();
        }
        _skills[i].ActivateSkill(this, anim, stats, player);
      }
      if (false == basicAttackExists)
      {
        if (_basicAttackSkill != null) 
        {
          Skills.Add(_basicAttackSkill);
          _basicAttackSkill.ActivateSkill(this, anim, stats, player); 
        }
        else
          Debug.LogError("Please add the basic attack skill to the skills");
      }

      Initialize();
    }

    /// <summary>
    /// Initializes player on Start() method.
    /// </summary>
    protected virtual void Initialize()
    {
      // Gets distance from target on start
      distanceFromTarget = ParaverseHelper.GetDistance(transform.position, player.position);
      curSetSkillToCompleteTimer = setSkillToCompleteTimer;

      IsSkilling = false;
    }

    protected virtual void Update()
    {
      if (controller.IsDead) return;

      distanceFromTarget = ParaverseHelper.GetDistance(transform.position, player.position);
      IsBasicAttacking = anim.GetBool(StringData.IsBasicAttacking);

      // Only applies to mobs that are NOT the player 
      if (anim.GetBool(StringData.IsUsingSkill) && false == transform.tag.Equals(StringData.PlayerTag))
        IsSkilling = true;
      else
        IsSkilling = false;

      // Checks if mob is using any skill (including basic attack)
      if (IsSkilling || IsBasicAttacking)
        _isAttacking = true;
      else
        _isAttacking = false;


      MobSkill prevSkill = null;
      MobSkill curSkill = null;
      // Gets active skill to run update method for each skill 
      // Starts a safe timer to reset skill in case skill gets stuck in a state other than InActive
      for (int i = 0; i < _skills.Count; i++)
      {
        _skills[i].SkillUpdate();

        if (_activeSkill != null) prevSkill = _activeSkill;

        if (_skills[i].SkillState.Equals(SkillState.InUse))
        {
          curSkill = _skills[i];
          _activeSkill = curSkill; // Sets Active skill
        }
        if (_skills[i].SkillState.Equals(SkillState.InActive))
        {
          if (_activeSkill == _skills[i])
          {
            _activeSkill = null; // Sets Active skill to null
          }
        }
      }
      if (prevSkill != curSkill || prevSkill == null || curSkill == null)
      {
        curSetSkillToCompleteTimer = setSkillToCompleteTimer;
      }
      // Automatically set active skill to false due to potential bug
      else if (curSetSkillToCompleteTimer <= 0)
      {
        Debug.LogError("Potential bug with set skill: " + _activeSkill + " off! Please ensure the skill is appropriately set off in OnSkillComplete() method!");
        _activeSkill.SetSkillState(SkillState.InActive);
        _activeSkill = null;
      }
      else if (prevSkill.ID == curSkill.ID)
      {
        curSetSkillToCompleteTimer -= Time.deltaTime;
      }
    }

    #region Exclusive To Enemy Methods
    /// <summary>
    /// Returns the basic attack cooldown based on attack speed value in stats.
    /// </summary>
    /// <returns></returns>
    protected float GetBasicAttackCooldown()
    {
      return 1f / stats.AttackSpeed.FinalValue;
    }

    public void EnrageStats()
    {
      if (TryGetComponent(out stats))
      {
        stats.AttackDamage.AddMod(new((int)(stats.AttackDamage.FinalValue * 0.5f)));
        stats.MaxHealth.AddMod(new(stats.MaxHealth.FinalValue));
        stats.SetFullHealth();
        if (_basicAttackSkill)
        {
          _basicAttackSkill.Cooldown *= 0.25f;
        }
        stats.MoveSpeed.AddMod(new(2));
      }
    }
    #endregion

    /// <summary>
    /// Resets booelans when mob is interrupted during attack. 
    /// </summary>
    public virtual void OnAttackInterrupt()
    {
      _isAttackLunging = false;
      if (_basicAttackSkill != null) DisableBasicAttackCollider();
      IsSkilling = false;
      OnAttackInterrupted?.Invoke();
    }

    #region Animation Event Methods
    /// <summary>
    /// Enables basic weapon collider.
    /// </summary>
    protected void EnableBasicAttackCollider()
    {
      if (null != _basicAttackSkill.attackColliderGO)
        _basicAttackSkill.attackColliderGO.SetActive(true);

      OnEnableBasicAttackCollider?.Invoke();
    }

    /// <summary>
    /// Disables basic attack collider.
    /// </summary>
    protected void DisableBasicAttackCollider()
    {
      if (null != _basicAttackSkill.attackColliderGO)
        _basicAttackSkill.attackColliderGO.SetActive(false);

      _basicAttackSkill.SetSkillState(SkillState.Used);
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
      // Need to fix this for player as ActiveSkill is always active
      // Archers may hold an arrow which needs to be set to off/on when firing
      if (ActiveSkill.projData.projHeld != null)
        ActiveSkill.projData.projHeld.SetActive(false);

      Vector3 playerPos = new Vector3(player.position.x, player.position.y + 0.5f, player.position.z);
      Vector3 targetDir = (playerPos - transform.position).normalized;

      Quaternion lookRot;
      if (ActiveSkill.projData.projRotation == null)
        lookRot = Quaternion.LookRotation(targetDir);
      else
        lookRot = ActiveSkill.projData.projRotation.rotation;

      // Instantiate and initialize projectile
      GameObject go = Instantiate(ActiveSkill.projData.projPf, ActiveSkill.projData.projOrigin.position, lookRot);
      Projectile proj = go.GetComponent<Projectile>();
      proj.Init(this, targetDir, ActiveSkill.projData.projSpeed, ActiveSkill.scalingStatData);

      if (false == ActiveSkill.DisableSkillUponProjectileFiring)
        OnDisableSkillOneEvent();
    }

    /// <summary>
    /// Enables the projectile held by the mob.
    /// </summary>
    public void EnableHeldProjectile()
    {
      if (null == ActiveSkill)
      {
        Debug.LogError("No Active Skill!");
        return;
      }

      if (ActiveSkill.projData.projHeld == null)
      {
        Debug.LogError("There is no reference to the projHeld variable for skill: ." + ActiveSkill.name);
        return;
      }

      ActiveSkill.projData.projHeld.SetActive(true);
    }

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
    #endregion
  }
}
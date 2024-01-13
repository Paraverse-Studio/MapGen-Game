using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using ParaverseWebsite.Models;
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
    protected MobStats stats;
    // Used for specific speed altering skills 
    protected CharacterController _controller;
    protected PlayerController _player;

    // General Skill Info
    public string Name { get => _skillName; set => _skillName = value; }
    [SerializeField, Tooltip("Skill name.")]
    protected string _skillName = "";
    public SkillName SkillName;  // Required for identifying for database
    public int ID { get => _ID; set => _ID = value; }
    [SerializeField, Tooltip("Skill ID.")]
    protected int _ID = -1;
    public Sprite Image { get => _image; set => _image = value; }
    [SerializeField]
    protected Sprite _image = null;
    public string Description { get => _description; set => _description = value; }
    [SerializeField, TextArea(2, 3), Tooltip("Skill description.")]
    protected string _description = "";
    [SerializeField, Tooltip("Name of skill animation to play.")]
    protected string animName = "";
    [SerializeField, Tooltip("Required energy cost to execute skill.")]
    protected float cost = 0f;
    public bool IsBasicAttack => _isBasicAttack;
    [SerializeField, Tooltip("Will subscribe skill to skill execute listener. [if isBasicAttack = false]")]
    protected bool _isBasicAttack = false;
    [Tooltip("Will fetch attack collider GO. [if isMelee = true]")]
    public bool IsMelee => _isMelee;
    [SerializeField, Tooltip("Will get and initialize the attack collider script from the given attack collider object")]
    protected bool _isMelee;
    [Header("Uses Target Lock"), Tooltip("If this skill should force mob to face its target")]
    public bool usesTargetLock;
    [SerializeField, Tooltip("Speed of rotation during skill.")]
    protected float rotSpeed = 110f;
    public bool DisableSkillUponProjectileFiring => enableForDurationBasedProjectileSkills;
    [SerializeField, Tooltip("Use for skills that fire a projectile but required the mob to stay in an animation for a set duration.")]
    protected bool enableForDurationBasedProjectileSkills = false;

    // Skill Attributes
    public float MinRange => _minRange;
    [SerializeField, Tooltip("Min skill range value.")]
    protected float _minRange = 0f;
    public float MaxRange => _maxRange;
    [SerializeField, Tooltip("Max skill range value.")]
    protected float _maxRange = 5f;
    public float Cooldown { get => _cooldown; set => _cooldown = value; }
    [SerializeField, Tooltip("Skill cooldown value.")]
    protected float _cooldown = 5f;
    public float CurCooldown { get => _curCooldown; set => _curCooldown = value; }
    protected float _curCooldown;
    public bool IsOffCooldown => _curCooldown <= 0;


    // Skill Additional Values
    [Header("Attack Collider Values")]
    public GameObject attackColliderGO;
    public AttackCollider attackCollider;
    [Header("Projectile Values")]
    public ProjectileData projData;
    [Header("Scaling Values")]
    public ScalingStatData scalingStatData;

    // Skill Condition Checks
    public bool TargetWithinRange { get { return IsInRange(); } }
    public bool HasEnergy => cost <= stats.CurEnergy;

    // Used to determine if skill is active/inactive
    // Only one skill can be active at a point in time
    //public bool skillOn { get; set; }
    public SkillState SkillState { get; set; }
    [SerializeField, Tooltip("")]
    protected float skillStateToCompleteTimer = 1f;
    protected float curSkillStateToCompleteTimer = 0f;

    public delegate void OnExecuteSkillDel();
    public event OnExecuteSkillDel OnExecuteSkillEvent;
    #endregion

    public void SetSkillState(SkillState state)
    {
      SkillState = state;
    }

    #region Private Methods

    /// <summary>
    /// Responsible for executing skill on button press.
    /// </summary>
    protected void Execute()
    {
      if (CanUseSkill())
      {
        SetSkillState(SkillState.InUse);
        OnExecuteSkillEvent?.Invoke();
        SubscribeAnimationEventListeners();
        ExecuteSkillLogic();
      }
    }
    #endregion

    #region Inheritable 
    /// <summary>
    /// Activates players skill ONLY. Needs to be used for EVERY skill inorder to activate skill when obtained.
    /// </summary>
    /// <param name="mob"></param>
    /// <param name="input"></param>
    /// <param name="anim"></param>
    /// <param name="stats"></param>
    /// <param name="target"></param>
    public virtual void ActivateSkill(PlayerCombat mob, PlayerInputControls input, Animator anim, MobStats stats, Transform target = null)
    {
      this.mob = mob;
      this.target = target;
      this.input = input;
      this.anim = anim;
      this.stats = stats;
      _curCooldown = 0f;
      if (mob.gameObject.CompareTag(StringData.PlayerTag) && _isBasicAttack == false)
        input.OnSkillOneEvent += Execute;
      SetSkillState(SkillState.InActive);

      if (null == attackColliderGO)
      {
        attackColliderGO = mob.AttackColliderGO;
      }
      attackColliderGO.SetActive(false);

      if (_isMelee)
      {
        attackCollider = attackColliderGO.GetComponent<AttackCollider>();
        attackCollider.Init(mob, scalingStatData);
      }
      mob.OnAttackInterrupted += InterruptSkill;
    }

    /// <summary>
    /// Required inorder to activate skill to be used. 
    /// </summary>
    /// <param name="mob"></param>
    /// <param name="anim"></param>
    /// <param name="stats"></param>
    /// <param name="target"></param>
    public virtual void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
    {
      this.mob = mob;
      this.target = target;
      this.anim = anim;
      this.stats = stats;
      _curCooldown = _cooldown;
      SetSkillState(SkillState.InActive);

      if (null == attackColliderGO)
      {
        Debug.LogWarning(gameObject.name + " doesn't have an attack collider.");
        return;
      }
      attackCollider = attackColliderGO.GetComponent<AttackCollider>();
      attackCollider.Init(mob, scalingStatData);
    }

    /// <summary>
    /// Required inorder to deactivate skill when player dies or skill is no longer required.[REQUIRED FOR PLAYER SKILLS ONLY]
    /// </summary>
    /// <param name="input"></param>
    public virtual void DeactivateSkill(PlayerInputControls input)
    {
      if (mob.tag.Equals(StringData.PlayerTag))
      {
        input.OnSkillOneEvent -= Execute;
        mob.OnAttackInterrupted -= InterruptSkill;
      }
    }

    /// <summary>
    /// Registers the skills animation events to the animation event methods in combat script.
    /// </summary>
    public virtual void SubscribeAnimationEventListeners()
    {
      mob.OnDisableSkillOneEvent += OnSkillComplete;
    }

    /// <summary>
    /// Unsubscribes the skills animation events to the animation event methods in combat script.
    /// </summary>
    public virtual void UnsubscribeAnimationEventListeners()
    {
      mob.OnDisableSkillOneEvent -= OnSkillComplete;
    }

    /// <summary>
    /// Contains all methods required to run in Update within MobCombat script.
    /// </summary>
    public virtual void SkillUpdate()
    {
      if (null != target && mob.IsBasicAttacking == false && mob.IsSkilling == false)
        Execute();

      SkillStateManager();
      CooldownHandler();
    }

    protected virtual void SkillStateManager()
    {
      if (SkillState.Equals(SkillState.InUse))
        TargetLockDuringSkill();

      if (SkillState.Equals(SkillState.Used))
      {
        // change skill state to complete after a set period of delay
        if (target != null)
          RotateToTarget();
        if (curSkillStateToCompleteTimer <= 0)
        {
          SetSkillState(SkillState.InActive);
          curSkillStateToCompleteTimer = skillStateToCompleteTimer;
        }
        else
        {
          curSkillStateToCompleteTimer -= Time.deltaTime;
        }
      }
    }

    protected virtual void InterruptSkill()
    {
      //skillOn = false;
      SetSkillState(SkillState.InActive);
    }

    public void RefundCooldown(float refund)
    {
      _curCooldown -= refund;
    }

    /// <summary>
    /// Keeps mob targetted on the target during skill, only when usesTargetLock boolean is set to true
    /// </summary>
    protected virtual void TargetLockDuringSkill()
    {
      if (SkillState.Equals(SkillState.InActive)) return;

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

    protected void RotateToTarget()
    {
      Vector3 lookDir = (target.transform.position - mob.transform.position).normalized;
      Quaternion lookRot = Quaternion.LookRotation(lookDir);
      mob.transform.rotation = Quaternion.Slerp(mob.transform.rotation, lookRot, rotSpeed * Time.deltaTime);
    }

    /// <summary>
    /// This method is to be run everytime a skill is executed.
    /// </summary>
    protected virtual void ExecuteSkillLogic()
    {
      mob.IsSkilling = true;
      SetSkillState(SkillState.InUse);
      anim.SetBool(StringData.IsUsingSkill, true);
      stats.UpdateCurrentEnergy(-cost);
      anim.Play(animName);
      curSkillStateToCompleteTimer = skillStateToCompleteTimer;
    }

    /// <summary>
    /// Required to run this method to manually set the mobs skill off. 
    /// </summary>
    protected virtual void OnSkillComplete()
    {
      // do the following for non player mobs
      if (input == null)
      {
        SetSkillState(SkillState.Used);
      }
      else
      {
        SetSkillState(SkillState.InActive);
      }
      anim.SetBool(StringData.IsUsingSkill, false);
      _curCooldown = _cooldown * (1.0f - (stats.CooldownReduction.FinalValue / 100.0f));

      UnsubscribeAnimationEventListeners();
    }

    /// <summary>
    /// Handles skill cooldown.
    /// </summary>
    protected virtual void CooldownHandler()
    {
      if (_curCooldown > 0)
      {
        _curCooldown -= Time.deltaTime;
      }
      _curCooldown = Mathf.Clamp(_curCooldown, 0f, _cooldown);
    }

    /// <summary>
    /// Returns true if skill conditions are met. 
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanUseSkillForPlayer()
    {
      if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && anim.GetBool(StringData.IsBasicAttacking) == false)
        return true;

      return false;
    }

    /// <summary>
    /// Returns true if skill conditions are met. 
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanUseSkill()
    {
      if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && anim.GetBool(StringData.IsBasicAttacking) == false && mob.ActiveSkill == null ||
        IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && anim.GetBool(StringData.IsBasicAttacking) == false && input != null) // For player Active skill is always active
        return true;

      return false;
    }

    /// <summary>
    /// Returns total damage applied by skill using mob stats along with the skills scaling data. 
    /// </summary>
    /// <returns></returns>
    protected virtual float GetPowerAmount()
    {
      return scalingStatData.flatPower + (stats.AttackDamage.FinalValue * scalingStatData.attackScaling) + (stats.AbilityPower.FinalValue * scalingStatData.abilityScaling);
    }

    protected virtual bool IsInRange()
    {
      if (target == null) return true;

      float disFromTarget = ParaverseHelper.GetDistance(mob.transform.position, target.position);

      return disFromTarget >= _minRange && disFromTarget <= _maxRange;
    }
    #endregion
  }

  public enum SkillState
  {
    InActive,  // skill is fully completed, can move on to next skill
    InUse, // skill is in use
    Used, // skill is used, but still need to stay on skill
  };
}
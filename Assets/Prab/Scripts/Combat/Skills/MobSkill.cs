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
    protected MobStats stats;

    // General Skill Info
    public string Name { get => _skillName; set => _skillName = value; }
    [SerializeField, Tooltip("Skill name.")]
    protected string _skillName = "";
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

    // Skill Attributes
    public float MinRange => _minRange;
    [SerializeField, Tooltip("Min skill range value.")]
    protected float _minRange = 0f;
    public float MaxRange => _maxRange;
    [SerializeField, Tooltip("Max skill range value.")]
    protected float _maxRange = 5f;
    public float Cooldown => _cooldown; 
    [SerializeField, Tooltip("Skill cooldown value.")]
    protected float _cooldown = 5f;
    public float CurCooldown => _curCooldown;
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
    public bool skillOn { get; set; }

    public delegate void OnExecuteSkillDel();
    public event OnExecuteSkillDel OnExecuteSkillEvent;
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

    protected virtual void InterruptSkill()
    {
      skillOn = false;
    }

    /// <summary>
    /// Activates mobs skill ONLY
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

      if (null == attackColliderGO)
      {
        Debug.LogWarning(gameObject.name + " doesn't have an attack collider.");
        return;
      }
      attackCollider = attackColliderGO.GetComponent<AttackCollider>();
      attackCollider.Init(mob, scalingStatData);
    }

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
      mob.OnDisableSkillOneEvent += DisableSkill;
    }

    /// <summary>
    /// Unsubscribes the skills animation events to the animation event methods in combat script.
    /// </summary>
    public virtual void UnsubscribeAnimationEventListeners()
    {
      mob.OnDisableSkillOneEvent -= DisableSkill;
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

      if (skillOn)
        RotateToTarget();

      CooldownHandler();
    }

    public void RefundCooldown(float refund)
    {
      _curCooldown -= refund;
      Debug.Log("cur CD: " + _curCooldown);
    }

    protected virtual void RotateToTarget()
    {
      if (skillOn == false) return;

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

    /// <summary>
    /// Run this method everytime a skill is activated
    /// </summary>
    protected virtual void ExecuteSkillLogic()
    {
      mob.IsSkilling = true;
      skillOn = true;
      anim.SetBool(StringData.IsUsingSkill, true);
      _curCooldown = _cooldown * (1.0f - (stats.CooldownReduction.FinalValue / 100.0f));
      stats.UpdateCurrentEnergy(-cost);
      anim.Play(animName);
    }

    protected virtual void DisableSkill()
    {
      skillOn = false;
      anim.SetBool(StringData.IsUsingSkill, false);

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
    protected virtual bool CanUseSkill()
    {
      if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && anim.GetBool(StringData.IsBasicAttacking) == false)
        return true;

      return false;
    }

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

    #region Private Methods

    /// <summary>
    /// Responsible for executing skill on button press.
    /// </summary>
    protected void Execute()
    {
      if (CanUseSkill())
      {
        OnExecuteSkillEvent?.Invoke();
        SubscribeAnimationEventListeners();
        ExecuteSkillLogic();
      }
    }
    #endregion
  }
}
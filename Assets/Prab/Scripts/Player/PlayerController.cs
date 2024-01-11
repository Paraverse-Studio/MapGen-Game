using Paraverse.Helper;
using Paraverse.IK;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace Paraverse.Player
{
  [RequireComponent(typeof(CharacterController))]
  [RequireComponent(typeof(PlayerInputControls))]
  [RequireComponent(typeof(MobStats))]
  [RequireComponent(typeof(MobCombat))]
  public class PlayerController : MonoBehaviour, IMobController
  {
    #region Variables 
    // Important unity components
    public static PlayerController Instance;
    private Camera cam;
    private Animator anim;
    private CharacterController controller;
    private PlayerInputControls input;
    private HeadIK headIK;
    // Reference to the combat script
    private PlayerCombat combat;
    // Reference to the stats script
    IMobStats IMobController.Stats { get { return stats; } }
    private IMobStats stats;

    [Header("Movement Values"), Tooltip("The current speed of the mob")]
    private float curSpeed;
    [SerializeField, Tooltip("The rotation speed of the mob.")]
    private float rotSpeed = 10f;

    [Header("Jump Values")]
    //[SerializeField, Tooltip("The jump force of the mob.")]
    //private float jumpForce = 10f;
    [SerializeField, Range(0.1f, 1f), Tooltip("Raycast distance to detect is Grounded")]
    private float disToGroundCheck = 0.1f;
    //[SerializeField, Range(0, 2), Tooltip("Time required to wait in between each jump.")]
    //private float jumpCd = 0.5f;
    //private float curJumpCd = 0f;
    [SerializeField, Tooltip("Detect these layers to consider mob is grounded.")]
    private LayerMask groundedLayers;
    [SerializeField]
    private float jumpGravity = -40f;

    //[Header("Landing Avoidance")]
    //[SerializeField, Tooltip("Raycast distance to check for enemies below for land avoidance.")]
    //private float disToEnemyCheck = 0.5f;
    //[SerializeField, Tooltip("Avoid landing on these layers.")]
    //private LayerMask avoidLayers;
    //[SerializeField, Tooltip("Force applied to player movement to avoid the avoidLayers.")]
    //private float avoidanceForce = 10f;
    //[SerializeField, Tooltip("Downward force applied on the mob when landing off an avoidance object.")]
    //private float downwardAvoidanceForceRatio = 2f;

    [Header("Dive Values")]
    [SerializeField, Tooltip("The dive force of the mob.")]
    public float diveForce = 10f;
    [SerializeField, Range(0, 10), Tooltip("The max distance of dive.")]
    public float maxDiveRange = 3f;
    [SerializeField, Range(0, 3), Tooltip("The max duration of dive.")]
    public float maxDiveDuration = 1f;
    private float curDiveDuration;
    [SerializeField, Range(0, 2), Tooltip("Time required to wait in between each dive.")]
    private float diveCd = 0.5f;
    private float curDiveCd = 0f;
    public delegate void OnStartDiveDel();
    public event OnStartDiveDel OnStartDiveEvent;
    public delegate void OnEndDiveDel();
    public event OnEndDiveDel OnEndDiveEvent;

    [Header("Knockback Values")]
    private KnockBackEffect activeKnockBackEffect;
    private float disFromStartPos;

    [Header("Attack Dashing Values")]
    [SerializeField, Tooltip("The attack dashing force applied during basic attack.")]
    private float atkDashForce = 2f;
    [SerializeField, Tooltip("The attack dashing force applied during basic attack two.")]
    private float atkTwoDashForce = 2f;
    [SerializeField, Tooltip("The attack dashing force applied during basic attack three.")]
    private float atkThreeDashForce = 4f;

    [Header("Death Values")]
    private GameObject deathEffect;
    public delegate void OnDeathDel(Transform target);
    public event IMobController.OnDeathDel OnDeathEvent;

    // State Booleans
    public float JumpGravity { get { return jumpGravity; } set { jumpGravity = value; } }
    public Transform Transform { get { return transform; } }
    public bool IsInteracting { get { return _isInteracting; } }
    private bool _isInteracting = false;
    public bool IsBasicAttacking { get { return _isBasicAttacking; } }
    private bool _isBasicAttacking = false;
    public bool IsSkilling { get { return _isUsingSkill; } }
    private bool _isUsingSkill = false;
    public bool IsMoving { get { return _isMoving; } }
    private bool _isMoving = false;
    public bool IsGrounded { get { return _isGrounded; } }
    private bool _isGrounded = false;
    public bool IsAvoidingObjUponLanding { get { return _isAvoidingObjUponLanding; } }
    private bool _isAvoidingObjUponLanding = false;
    public bool IsDiving { get { return _isDiving; } }
    private bool _isDiving = false;
    public bool IsStaggered { get { return _isStaggered; } }
    private bool _isStaggered = false;
    public bool IsHardCCed { get { return _isHardCced; } }
    private bool _isHardCced = false;
    public bool IsSoftCCed { get { return _isSoftCced; } }
    private bool _isSoftCced = false;
    public bool IsUnstaggerable { get; }
    private bool _isInvulnerable = false;
    public bool IsDead { get { return _isDead; } }
    private bool _isDead = false;
    public Transform Target { get { return _target; } }
    private Transform _target;

    // Movement, Jump & Dive inputs and velocities
    private Vector3 goalDir;
    private Vector3 moveDir;
    private Vector3 jumpDir;
    private Vector3 diveDir;
    private Vector3 knockbackDir;
    // Gets the start positions
    private Vector3 diveStartPos;
    // Gets the controller horizontal and vertical inputs
    private float horizontal;
    private float vertical;
    #endregion

    #region Start & Update Methods
    private void Awake()
    {
      if (null == Instance)
        Instance = this;
      else
        Destroy(this);
    }

    private void Start()
    {
      if (cam == null) cam = Camera.main;
      if (anim == null) anim = GetComponentInChildren<Animator>();
      if (controller == null) controller = GetComponent<CharacterController>();
      if (input == null) input = GetComponent<PlayerInputControls>();
      if (combat == null) combat = GetComponent<PlayerCombat>();
      if (stats == null) stats = GetComponent<IMobStats>();
      if (headIK == null) headIK = GetComponent<HeadIK>();

      // Subscribes item pick up code to use item event listener
      input.OnUseItemOneEvent += UseItemOne;
      input.OnUseItemTwoEvent += UseItemTwo;
      input.OnUseItemThreeEvent += UseItemThree;
      input.OnUseItemFourEvent += UseItemFour;
      //input.OnJumpEvent += Jump;
      input.OnDiveEvent += Dive;
      input.OnTargetLockEvent += TargetLock;
    }

    private void Update()
    {
      _isMoving = Mathf.Abs(vertical) > 0 || Mathf.Abs(horizontal) > 0;
      _isGrounded = IsGroundedCheck();

      jumpDir.y -= Time.deltaTime;

      AnimationHandler();
      DeathHandler();
      if (_isDead) return;

      MovementHandler();
      RotationHandler();
      JumpHandler();
      //AvoidObjUponLand();
      DiveHandler();
      KnockbackHandling();
      AttackMovementHandler();
    }
    #endregion

    #region Helper Methods
    public float GetWalkSpeed()
    {
      return stats.MoveSpeed.FinalValue;
    }
    #endregion

    #region Movement Handler Methods
    private void AnimationHandler()
    {
      anim.SetFloat(StringData.Speed, moveDir.magnitude);
      anim.SetBool(StringData.IsGrounded, IsGrounded);
      anim.SetBool(StringData.IsDead, IsDead);
      anim.SetBool(StringData.IsDiving, IsDiving);
      anim.SetBool(StringData.IsKnockedBack, IsStaggered);
      _isInteracting = anim.GetBool(StringData.IsInteracting);
      _isBasicAttacking = anim.GetBool(StringData.IsBasicAttacking);
    }

    private void MovementHandler()
    {
      // Disables player movement during dive/
      if (_isDiving || _isStaggered || _isInteracting && !combat.CanComboAttackTwo && !combat.CanComboAttackThree && !_isUsingSkill) return;

      // Adjusts player speed based on state
      if (IsInteracting)
        curSpeed = 0f;
      else
        curSpeed = GetWalkSpeed();

      // Gets movement input values
      horizontal = this.input.MovementDirection.x;
      vertical = this.input.MovementDirection.y;

      // Player's raw input
      Vector3 input = new Vector3(horizontal, 0, vertical);

      // The matrix that holds our camera angle
      Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45f, 0));

      // Our camera angle's matrix applied to our input, to get input relative to camera
      goalDir = matrix.MultiplyPoint3x4(input);

      // Lerping user's existing movement force to the goal movement force
      moveDir = Vector3.Lerp(moveDir, goalDir, Time.deltaTime * rotSpeed);

      controller.Move(moveDir * curSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Can be pulled and used in other scripts to control movement
    /// </summary>
    /// <param name="moveSpeed"></param>
    public void ControlMovement(float moveSpeed)
    {
      // Gets movement input values
      horizontal = this.input.MovementDirection.x;
      vertical = this.input.MovementDirection.y;

      // Player's raw input
      Vector3 input = new Vector3(horizontal, 0, vertical);

      // The matrix that holds our camera angle
      Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45f, 0));

      // Our camera angle's matrix applied to our input, to get input relative to camera
      goalDir = matrix.MultiplyPoint3x4(input);

      // Lerping user's existing movement force to the goal movement force
      moveDir = Vector3.Lerp(moveDir, goalDir, Time.deltaTime * rotSpeed);

      controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    private void RotationHandler()
    {
      if (_isStaggered) return;

      // Rotates player to locked target when attacking 
      if (null != _target && _isBasicAttacking)
      {
        Vector3 targetDir = ParaverseHelper.GetPositionXZ(_target.position - transform.position).normalized;
        transform.forward = targetDir;
      }
      else if (moveDir != Vector3.zero)
      {
        Quaternion targetLook = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetLook, rotSpeed * Time.deltaTime);
      }
    }
    #endregion

    #region Jump Handler Methods
    /// <summary>
    /// Invokes jump action
    /// </summary>
    //private void Jump()
    //{
    //    if (_isStaggered || _isInteracting || _isAvoidingObjUponLanding) return;

    //    if (_isGrounded && curJumpCd >= jumpCd)
    //    {
    //        curJumpCd = 0f;
    //        jumpDir.y += Mathf.Sqrt(jumpForce * -GlobalValues.GravityModifier * GlobalValues.GravityForce);
    //        anim.Play(StringData.Jump);
    //    }
    //}

    /// <summary>
    /// Handles jump movement and variables in Updat().
    /// </summary>
    private void JumpHandler()
    {
      ApplyGravity();

      //curJumpCd += Time.deltaTime;
      //curJumpCd = Mathf.Clamp(curJumpCd, 0, jumpCd);

      controller.Move(jumpDir * Time.deltaTime);
    }

    /// <summary>
    /// Responsible for applying gravity to player.
    /// </summary>
    private void ApplyGravity()
    {
      // Ensures player remains grounded when grounded 
      if (jumpDir.y < 0 && _isGrounded)
      {
        jumpDir.y = 0f;
      }

      // Applies gravity and jump movement
      jumpDir.y += jumpGravity * Time.deltaTime;
    }

    /// <summary>
    /// Returns true if player is grounded.
    /// </summary>
    /// <returns></returns>
    private bool IsGroundedCheck()
    {
      Vector3 origin = transform.position;
      Vector3 dir = -transform.up;

      if (Physics.Raycast(origin, dir * disToGroundCheck, disToGroundCheck, groundedLayers))
      {
        _isAvoidingObjUponLanding = false;
        return true;
      }

      return false;
    }

    /// <summary>
    /// Checks if mob is landing on an avoidance layer object, if so, sets avoidLandingOn as true. 
    /// </summary>
    //private void AvoidObjUponLand()
    //{
    //    RaycastHit hit;
    //    Vector3 origin = transform.position;
    //    Vector3 dir = -transform.up;

    //    if (Physics.SphereCast(origin, controller.radius, dir * disToEnemyCheck, out hit, disToEnemyCheck, avoidLayers))
    //    {
    //        _isAvoidingObjUponLanding = true;
    //    }
    //    LandingAvoidanceHandler();
    //}

    ///// <summary>
    ///// Handles avoiding landing
    ///// </summary>
    //private void LandingAvoidanceHandler()
    //{
    //    Vector3 offDis = new Vector3(transform.forward.x, -downwardAvoidanceForceRatio, transform.forward.z);
    //    if (_isAvoidingObjUponLanding)
    //    {
    //        controller.Move(offDis * avoidanceForce * Time.deltaTime);
    //    }

    //    // Apply avoidance force if mob is not grounded within isNotGroundedMaxDur
    //    //if (_isGrounded == false)
    //    //{
    //    //    isNotGroundedDur += Time.deltaTime;
    //    //    if (isNotGroundedDur >= isNotGroundedMaxDur)
    //    //        _isAvoidingObjUponLanding = true;
    //    //}
    //    //else
    //    //{
    //    //    isNotGroundedDur = 0f;
    //    //}
    //}
    #endregion

    #region Dive Methods
    /// <summary>
    /// Invokes dive action
    /// </summary>
    private void Dive()
    {
      if (_isStaggered || _isDiving || combat.IsAttackLunging || combat.IsSkilling) return;

      if (_isGrounded && curDiveCd >= diveCd * (1.0f - (stats.CooldownReduction.FinalValue / 100.0f)))
      {
        stats.ConsumeDiveEnergy();
        _isDiving = true;
        curDiveCd = 0f;
        curDiveDuration = 0f;
        diveStartPos = transform.position;
        controller.detectCollisions = false;

        // Allows player to dash when stand still
        if (_isMoving == false)
        {
          diveDir = new Vector3(transform.forward.x, 0f, transform.forward.z);
        }
        else
        {
          diveDir = new Vector3(goalDir.x, 0f, goalDir.z);
        }
        anim.Play(StringData.Dive);
        OnStartDiveEvent?.Invoke();
      }
    }

    /// <summary>
    /// Handles dive movement and variables in Update().
    /// </summary>
    private void DiveHandler()
    {
      if (_isDiving)
      {
        // Updates mob position and dive timer
        float diveRange = ParaverseHelper.GetDistance(transform.position, diveStartPos);
        curDiveDuration += Time.deltaTime;

        // Moves the mob in the move direction
        controller.Move(diveDir * diveForce * Time.deltaTime);

        // Stops dive when conditions met
        if (diveRange >= maxDiveRange || curDiveDuration >= maxDiveDuration)
        {
          controller.detectCollisions = true;
          _isDiving = false;
          OnEndDiveEvent?.Invoke();
          return;
        }
      }

      curDiveCd += Time.deltaTime;
      curDiveCd = Mathf.Clamp(curDiveCd, 0, diveCd);
    }

    private void TargetLock()
    {
      _target = SelectableSystem.Instance.ToggleSelect();
      combat.Target = _target;

      //headIK.SetLookAtObj(_target);
    }

    #endregion

    #region KnockBack Methods
    /// <summary>
    /// Applies knock back by passing the knockback effect
    /// </summary>
    public void ApplyKnockBack(Vector3 mobPos, KnockBackEffect effect)
    {
      if (IsUnstaggerable) return;

      combat.OnAttackInterrupt();
      Vector3 impactDir = (transform.position - mobPos).normalized;
      knockbackDir = new Vector3(impactDir.x, 0f, impactDir.z);
      activeKnockBackEffect = effect;
      effect.startPos = transform.position;
      anim.Play(StringData.Hit);
    }

    /// <summary>
    /// Handles knock back by applying force until either the target distance or duration condition is met.
    /// </summary>
    private void KnockbackHandling()
    {
      if (null != activeKnockBackEffect)
      {
        _isStaggered = true;
        // Moves the mob in the move direction
        controller.Move(knockbackDir * activeKnockBackEffect.knockForce * Time.deltaTime);

        activeKnockBackEffect.maxKnockbackDuration -= Time.deltaTime;
        disFromStartPos = ParaverseHelper.GetDistance(activeKnockBackEffect.startPos, transform.position);
        if (disFromStartPos >= activeKnockBackEffect.maxKnockbackRange || activeKnockBackEffect.maxKnockbackDuration <= 0)
        {
          _isStaggered = false;
          activeKnockBackEffect = null;
        }
      }
    }

    /// <summary>
    /// Plays hit animation
    /// </summary>
    public void ApplyHitAnimation()
    {
      anim.Play(StringData.Hit);
    }

    #endregion

    #region Attack Movement
    private void AttackMovementHandler()
    {
      if (combat.IsAttackLunging && combat.BasicAttackComboIdx == 0)
      {
        controller.Move(transform.forward * atkThreeDashForce * Time.deltaTime);
      }
      else if (combat.IsAttackLunging && combat.BasicAttackComboIdx == 2)
      {
        controller.Move(transform.forward * atkTwoDashForce * Time.deltaTime);
      }
      else if (combat.IsAttackLunging)
      {
        controller.Move(transform.forward * atkDashForce * Time.deltaTime);
      }
    }
    #endregion

    #region Death Handler Methods
    /// <summary>
    /// Handles player death based on conditions (HP, _isDead boolean).
    /// </summary>
    private void DeathHandler()
    {
      if (stats.CurHealth <= 0 && _isDead == false)
      {
        _isDead = true;
        Death();
        OnDeathEvent?.Invoke(transform);
      }
    }

    /// <summary>
    /// Runs upon player death.
    /// </summary>
    private void Death()
    {
      controller.detectCollisions = false;
      anim.Play(StringData.Death);

      horizontal = 0f;
      vertical = 0f;
    }

    private void ResetAllCC()
    {
      _isInteracting = false;
      _isSoftCced = false;
      _isBasicAttacking = false;
      _isDiving = false;
      _isHardCced = false;
      _isInvulnerable = false;
      _isStaggered = false;
      _isUsingSkill = false;
      activeKnockBackEffect = null;
    }

    /// <summary>
    /// Prepares player for next round  
    /// </summary>
    public void ResetPlayerRound()
    {
      controller.detectCollisions = true;
      _isDead = false;
      // also reset all _isInteracting, _knockedBack, etc. Basically all types of CC  ( @ PRAB )
      ResetAllCC();
      //stats.ResetStats(); March 7, not needed, player shouldn't get a free heal between rounds
      anim.Play(StringData.Idle);
    }

    /// <summary>
    /// Resets player completely by removing effects or anyother attributes that may need to be reset
    /// </summary>
    public void FactoryResetPlayerOnDeath()
    {
      combat.DeactivateEffects();
      stats.ResetStats();
      combat.DeactivateSkill();
      GameLoopManager.Instance.FactoryResetSessionStats();
      Debug.Log("Factory Reset");
    }
    #endregion

    #region Item Interaction Methods
    private void UseItemOne()
    {
      Debug.Log("Item One Used");
    }

    private void UseItemTwo()
    {
      Debug.Log("Item Two Used");
    }

    private void UseItemThree()
    {
      Debug.Log("Item Three Used");
    }

    private void UseItemFour()
    {
      Debug.Log("Item Four Used");
    }
    #endregion
  }
}
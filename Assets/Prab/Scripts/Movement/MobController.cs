using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace Paraverse.Mob.Controller
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MobCombat))]
    [RequireComponent(typeof(MobStats))]
    [RequireComponent(typeof(StatusEffectManager))]
    [RequireComponent(typeof(MobHealthBar))]
    [RequireComponent(typeof(Selectable))]
    public class MobController : MonoBehaviour, IMobController
    {
        #region Variables
        // Important unity components
        protected NavMeshAgent nav;
        protected Animator anim;
        protected CharacterController controller;
        // Reference to the combat script
        protected IMobCombat combat;
        protected StatusEffectManager statusEffectManager;
        // Reference to the stats script
        IMobStats IMobController.Stats { get { return stats; } }
        protected IMobStats stats;

        [Header("Movement Values"), Tooltip("The current speed of the mob")]
        protected float curMoveSpeed;
        [SerializeField, Range(0, 1), Tooltip("The walk speed of the mob.")]
        protected float walkSpeedRatio = 1f;
        [SerializeField, Range(1, 10), Tooltip("The sprint speed of the mob.")]
        protected float sprintSpeedRatio = 2.5f;
        [SerializeField, Tooltip("The rotation speed of the mob.")]
        protected float rotSpeed = 100f;

        [Header("Patrol State Values")]
        [SerializeField, Tooltip("The waypoints the mob is going to patrol between.")]
        protected Transform[] wps;
        protected int curWPIdx;
        [SerializeField, Range(0, 10), Tooltip("The minimum wait duration for the mob at each waypoint.")]
        protected float minWaitDur = 2f;
        [SerializeField, Range(0, 20), Tooltip("The maximum wait duration for the characther at each waypoint.")]
        protected float maxWaitDur = 5f;
        [Tooltip("The wait duration for the mob, calculated from a random value between minWaitDuration and maxWaitDuration.")]
        protected float startWaitDur;
        [Tooltip("The current wait timer, once it reaches the startWaitDuration, mob will proceed to next waypoint.")]
        private float curWaitTimer;
        [SerializeField, Tooltip("The distance at which the mob stops from its target waypoint.")]
        protected float accurayWP = 2f;
        [SerializeField, Tooltip("The distance at which the mob stops from its current target.")]
        protected float stoppingDistance = 2f;

        // Variables for detecting unreachable wps
        protected Vector3 curPos;
        protected Vector3 prevPos;
        protected float unreachableWpCheckTimer = 0f;

        [Header("Pursue State Values")]
        [SerializeField, Tooltip("The range at which the mob can detect targets.")]
        protected float detectionRadius = 5f;
        [SerializeField, Tooltip("The range at which the mob will stop pursuing target.")]
        protected float pursueRadius = 7f;
        [Tooltip("The tag of the target that the mob can detect and pursue.")]
        protected string targetTag = StringData.PlayerTag;
        [Tooltip("The targets transform")]
        protected Transform pursueTarget;
        protected IMobController playerController;
        [Tooltip("Set to true once target is detected.")]
        protected bool targetDetected = false;
        [SerializeField, Tooltip("Uses min basic attack range as default pursue range.")]
        protected bool customPursueRange = false;
        [SerializeField, Tooltip("Define the range at which the mob begins to pursue target.")]
        protected float pursueRange = 10f;

        [Header("Strafe Values")]
        [SerializeField, Tooltip("Allow mob strafing.")]
        protected bool _isStrafer = false;
        private bool isStrafing = false;    // Mob is looking for a strafe location
        private bool isStrafingToPoint = false; // Mob is strafing to a determined strafe location
        [SerializeField]
        private Transform strafePoint; // Updated strafe pos mob when strafing
        [SerializeField, Tooltip("How close should mob approach strafe position.")]
        protected float strafeAccuracy = 1f;

        [Header("Attack Dash Values")]
        [SerializeField, Tooltip("The attack dashing force applied during basic attack.")]
        protected float atkDashForce = 2f;

        [Header("Knockback Values")]
        protected Vector3 knockbackDir;
        [Tooltip("Active knock back applied to mob.")]
        protected KnockBackEffect activeKnockBackEffect;
        float disFromStartPos;

        [Header("Jump Values")]
        [SerializeField]
        protected float jumpLateralForce = 5f;
        [SerializeField]
        protected float jumpHeightForce = 5f;
        protected Vector3 landPos;
        [SerializeField]
        protected float isGroundedCheckRange = 0.5f;
        protected Vector3 jumpDir;
        private float curAirCheckTimer = 0.5f;
        private float jumpCheckTimer = 0.5f;
        [SerializeField]
        protected LayerMask groundLayer;
        public delegate void OnLandDel();
        public event OnLandDel OnLandEvent;

        [Header("Fly Values")]
        [SerializeField]
        protected float flyUpForce = 5f;
        [SerializeField]
        protected float flyDownForce = 8f;
        protected Vector3 flyDir;
        public bool isFlyingUp = false;
        public bool isFlyingDown = false;

        [Header("Fall Check")]
        [SerializeField, Tooltip("Checks if mob should fall off map.")]
        protected float checkFallRange = 2f;
        [SerializeField, Tooltip("Timer for enemy death when falling off map.")]
        protected float maxDeathTimerUponFall = 2f;
        protected float deathTimerUponFall;
        [SerializeField]
        protected float fallForce = 0.5f;

        [Header("Death Values")]
        [SerializeField]
        protected GameObject deathEffect;
        public delegate void OnDeathDel(Transform target);
        public event IMobController.OnDeathDel OnDeathEvent;

        // Reference to mob state
        [SerializeField, Tooltip("The current general state of the mob.")]
        protected MobState _curMobState;
        public MobState CurMobState { get { return _curMobState; } }

        // State Booleans 
        public Transform Transform { get { return transform; } }
        public bool IsInteracting { get { return _isInteracting; } }
        protected bool _isInteracting = false;
        public bool IsStaggered { get { return _isStaggered; } }
        protected bool _isStaggered = false;
        public bool IsHardCCed { get { return _isHardCced; } }
        protected bool _isHardCced = false;
        public bool IsSoftCCed { get { return _isSoftCced; } }
        protected bool _isSoftCced = false;
        public bool IsFalling { get { return _isFalling; } }
        protected bool _isFalling = false;
        public bool IsJumping { get { return _isJumping; } }
        protected bool _isJumping = false;
        public bool IsFlying { get { return _isFlying; } }
        protected bool _isFlying = false;
        public bool IsGrounded { get { return _isGrounded; } }
        public bool _isGrounded = true;
        public bool IsUnstaggerable { get { return _isUnstaggerable; } }
        [SerializeField]
        protected bool _isUnstaggerable = false;
        public bool IsDead { get { return _isDead; } }
        protected bool _isDead = false;

        public IMobController Target { get { return _target; } }
        protected IMobController _target;
        #endregion

        #region Start & Update Methods
        protected virtual void Start()
        {
            if (nav == null) nav = GetComponent<NavMeshAgent>();
            if (anim == null) anim = GetComponent<Animator>();
            if (controller == null) controller = GetComponent<CharacterController>();
            if (combat == null) combat = GetComponent<IMobCombat>();
            if (stats == null) stats = GetComponent<IMobStats>();
            if (statusEffectManager == null) statusEffectManager = GetComponent<StatusEffectManager>();
            if (strafePoint == null && _isStrafer)
            {
                GameObject objToSpawn = new GameObject(StringData.StrafePoint);
                objToSpawn.transform.SetParent(transform);
                strafePoint = objToSpawn.AddComponent<SphereCollider>().transform;
            }
            
            // Ensure basic attack range is >= to stopping distance
            if (combat.BasicAtkRange < stoppingDistance)
            {
                stoppingDistance = combat.BasicAtkRange;
                Debug.LogWarning(transform.name + " cannot have basic attack range lower than its stopping distance.");
            }

            // Ensures pursue range is greater than detection range
            if (pursueRadius <= detectionRadius)
            {
                pursueRadius = detectionRadius + 3f;
                Debug.LogWarning(transform.name + "'s pursue radius should NOT be less than its detection radius!");
            }

            // Checks if waypoint gameobjects have been referenced 
            if (wps.Length <= 0)
            {
                Debug.LogWarning("Mob has no waypoint references!");
            }

            Initialize();
        }

        private void Update()
        {
            _isInteracting = anim.GetBool(StringData.IsInteracting);

            DeathHandler();
            if (_isDead) return; 
            StateHandler();
            StrafeHandler();
            CCHandler();
            KnockbackHandler();
            JumpHandler();
            FlyUpHandler();
            FlyDownHandler();
            AnimatorHandler();
        }
        #endregion

        #region State Methods
        /// <summary>
        /// Responsible for handling and managing mob states.
        /// </summary>
        protected virtual void StateHandler()
        {
            nav.speed = curMoveSpeed;

            // Ensures nav is enabled after switching it off
            if (_isStaggered == false && combat.IsAttackLunging == false && IsFlying == false && IsJumping == false)
                nav.enabled = true;

            if (_isInteracting && IsFalling == false)
            {
                if (combat.IsAttackLunging)
                {
                    CleanseStagger();
                    AttackMovementHandler();
                }

                curMoveSpeed = 0f;
                return;
            }

            if (nav.enabled == false) return;

            if (TargetDetected() && combat.IsInCombat == false && combat.CanBasicAtk == false && playerController.IsDead == false)
            {
                PursueTarget();
                SetGeneralState(MobState.Pursue);
            }
            else if (TargetDetected() && playerController.IsDead == false)
            {
                CombatHandler();
                SetGeneralState(MobState.Combat);
            }
            else
            {
                nav.updateRotation = true;
                Patrol();
                SetGeneralState(MobState.Patrol);
            }
        }

        private void CCHandler()
        {
            if (_isHardCced)
            {
                HardCCHandler();
                curMoveSpeed = 0f;
            }
            else if (_isSoftCced)
            {
                SoftCCHandler();
                curMoveSpeed = 0f;
            }
            else if (_isStaggered)
            {
                curMoveSpeed = 0f;
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Responsible for handling mob animation.
        /// </summary>
        private void AnimatorHandler()
        {
            anim.SetFloat(StringData.Speed, curMoveSpeed);
            if (_curMobState.Equals(MobState.Pursue) && curMoveSpeed != 0)
                anim.SetBool(StringData.IsSprinting, true);
            else
                anim.SetBool(StringData.IsSprinting, false);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Initializes the mob upon Start().
        /// </summary>
        private void Initialize()
        {
            SetRandomWaypoint();
            nav.stoppingDistance = 0f;
            nav.acceleration = 999f;
            nav.autoBraking = false;

            playerController = GameObject.FindGameObjectWithTag(targetTag).GetComponent<IMobController>();
            pursueTarget = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
        }

        /// <summary>
        /// Only set state in StateHandler method to avoid bugs related to player's current state.
        /// </summary>
        /// <param name="state"></param>
        private void SetGeneralState(MobState state)
        {
            _curMobState = state;
        }

        /// <summary>
        /// Returns the calculated value from patrolSpeedRatio and movement Speed stat.
        /// </summary>
        /// <returns></returns>
        private float GetPatrolSpeed()
        {
            return walkSpeedRatio * stats.MoveSpeed.FinalValue;
        }

        /// <summary>
        /// Returns the calculated value from pursueSpeedRatio and movement Speed stat.
        /// </summary>
        /// <returns></returns>
        private float GetPursueSpeed()
        {
            return sprintSpeedRatio * stats.MoveSpeed.FinalValue;
        }
        #endregion

        #region Patrolling Logic
        /// <summary>
        /// Responsible for mob patrolling. 
        /// </summary>
        private void Patrol()
        {
            // Mob stays stationary when wps set to 0
            if (wps.Length <= 0)
            {
                nav.isStopped = true;
                return;
            }

            // Disregards the y position for distance calculation
            Vector3 mobPos = ParaverseHelper.GetPositionXZ(transform.position);
            Vector3 targetPos = ParaverseHelper.GetPositionXZ(wps[curWPIdx].position);

            float distanceFromWP = ParaverseHelper.GetDistance(mobPos, targetPos);

            if (distanceFromWP <= accurayWP)
            {
                nav.isStopped = true;
                if (curWaitTimer >= startWaitDur)
                {
                    SetRandomWaypoint();
                }
                else
                {
                    curMoveSpeed = 0f;
                    nav.isStopped = true;
                    curWaitTimer += Time.deltaTime;
                }
                unreachableWpCheckTimer = 0f;
            }
            else
            {
                curMoveSpeed = GetPatrolSpeed();
                nav.isStopped = false;

                // Checks if waypoint is unreachable
                UpdateUnreachableWaypointPosition();
                unreachableWpCheckTimer += Time.deltaTime;
            }
            nav.SetDestination(wps[curWPIdx].position);
        }

        /// <summary>
        /// Checks if waypoint is unreachable by mob, if so, updates waypoint to current position.
        /// </summary>
        private void UpdateUnreachableWaypointPosition()
        {
            curPos = transform.localPosition;
            if (curPos == prevPos && unreachableWpCheckTimer > 1f)
            {
                wps[curWPIdx].transform.position = transform.position;
                unreachableWpCheckTimer = 0f;
                //Debug.Log(transform.name + "'s wp is out of reach! Updated out of reach wp to current position: " + transform.position);
            }
            prevPos = curPos;
        }

        /// <summary>
        /// Sets the next waypoint as the destination and resets the current waypoint timer.
        /// </summary>
        private void SetRandomWaypoint()
        {
            if (wps.Length <= 0) return;
            curWaitTimer = 0f;
            nav.isStopped = false;
            curMoveSpeed = GetPatrolSpeed();
            startWaitDur = Random.Range(minWaitDur, maxWaitDur);
            int prevVal = curWPIdx;
            int safetyCounter = 0;

            // Prevents mob from going to current waypoint
            while (prevVal == curWPIdx && safetyCounter < 1000)
            {
                curWPIdx = Random.Range(0, wps.Length - 1);
                safetyCounter++;
                if (safetyCounter >= 1000)
                    throw new System.Exception("DANGER: While loop safety triggered!!");
            }
            nav.SetDestination(wps[curWPIdx].position);
        }
        #endregion 

        #region Pursue Target Logic
        /// <summary>
        /// Responsible for handling mob pursuing logic.
        /// </summary>
        private void PursueTarget()
        {
            if (pursueTarget != null)
            {
                float distanceFromTarget = ParaverseHelper.GetDistance(transform.position, pursueTarget.position);

                bool canPursue = false;
                if (distanceFromTarget <= combat.BasicAttackSkill.MinRange && customPursueRange == false)
                    canPursue = true;
                else if (distanceFromTarget <= pursueRange && customPursueRange)
                    canPursue = true;
                else
                    canPursue = false;

                if (canPursue)
                {
                    if (isStrafing && isStrafingToPoint == false)
                    {
                        float strafeDistance = 8f;

                        Vector3 strafePos = Random.insideUnitCircle.normalized * strafeDistance;

                        Vector3 origin = transform.position + new Vector3(0, 1f, 0);
                        Vector3 groundCheckDis = -transform.up * 3f;

                        if (Physics.Raycast(origin, strafePos, out RaycastHit hit, strafeDistance))
                        {
                            nav.isStopped = true;
                            curMoveSpeed = 0f;
                            nav.ResetPath();
                        }
                        else
                        {
                            if (Physics.Raycast(origin + strafePos, groundCheckDis, out hit, 3f))
                            {
                                nav.isStopped = false;
                                curMoveSpeed = GetPursueSpeed();
                                strafePoint.position = hit.point;
                                nav.SetDestination(strafePoint.position);
                                isStrafingToPoint = true;
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (isStrafingToPoint) return;
                        nav.isStopped = true;
                        curMoveSpeed = 0f;
                        transform.rotation = ParaverseHelper.FaceTarget(transform, pursueTarget, rotSpeed);
                        nav.SetDestination(pursueTarget.position);
                    }
                }
                else if (distanceFromTarget <= combat.BasicAttackSkill.MaxRange && distanceFromTarget > combat.BasicAttackSkill.MinRange && isStrafingToPoint == false)
                {
                    nav.isStopped = true;
                    curMoveSpeed = 0f;
                    transform.rotation = ParaverseHelper.FaceTarget(transform, pursueTarget, rotSpeed);
                    nav.SetDestination(pursueTarget.position);
                }
                else
                {
                    nav.updateRotation = true;
                    nav.isStopped = false;
                    curMoveSpeed = GetPursueSpeed();
                    nav.SetDestination(pursueTarget.position);
                    isStrafingToPoint = false;
                }
            }
        }

        /// <summary>
        /// Returns true if target was detected within detectionRange (and detectionRadius when useRaycastDetection is enabled).
        /// Manages the detection method via useRaycastDetection boolean.
        /// </summary>
        /// <returns></returns>
        private bool TargetDetected()
        {
            return WithinDetectionRadius();
        }

        /// <summary>
        /// Simple detection method which checks whether target is within detection radius. 
        /// Continues returning true until the mob and target distance is double the detection radius.
        /// </summary>
        /// <returns></returns>
        private bool WithinDetectionRadius()
        {
            if (null == pursueTarget)
            {
                Debug.LogError("Pursue target cannot be null.");
                return false;
            }

            float distanceFromTarget = ParaverseHelper.GetDistance(transform.position, pursueTarget.position);

            if (distanceFromTarget <= detectionRadius)
            {
                targetDetected = true;
                return true;
            }
            else if (targetDetected && distanceFromTarget <= pursueRadius)
            {
                return true;
            }
            else
            {
                targetDetected = false;
                return false;
            }
        }
        #endregion

        #region Combat Logic
        /// <summary>
        /// Responsible for handling mob combat.
        /// </summary>
        private void CombatHandler()
        {
            curMoveSpeed = 0;
            nav.isStopped = true;
            transform.rotation = ParaverseHelper.FaceTarget(transform, pursueTarget, rotSpeed);

            //// Ensures mob is looking at the target before attacking
            //Vector3 dir = (pursueTarget.position - transform.position).normalized;
            //dir = ParaverseHelper.GetPositionXZ(dir);
            //Quaternion lookRot = Quaternion.LookRotation(dir);
            //float angle = Quaternion.Angle(transform.rotation, lookRot);

            //if (angle <= 0)
            //    combat.BasicAttackSkill.ExecuteBasicAttack();
        }

        public void ApplyHitAnimation()
        {
            anim.Play(StringData.Hit);
        }
        #endregion

        #region Strafe Logic
        private void StrafeHandler()
        {
            if (_isStrafer == false) return;

            float distanceFromTarget = ParaverseHelper.GetDistance(transform.position, pursueTarget.position);
            float distanceFromStrafePoint = ParaverseHelper.GetDistance(transform.position, strafePoint.position);

            // Ensures mob strafes even when attacked and doesn't freeze
            if (distanceFromStrafePoint <= strafeAccuracy)
            {
                isStrafingToPoint = false;
                isStrafing = false;
            }
            else if (isStrafing && _curMobState.Equals(MobState.Pursue) && nav.enabled)
            {
                nav.isStopped = false;
                curMoveSpeed = GetPursueSpeed();
                nav.SetDestination(strafePoint.position);
            }

            // Handles mob strafing
            if (distanceFromTarget > combat.BasicAttackSkill.MinRange && isStrafingToPoint == false)
            {
                isStrafing = false;
            }
            else if (distanceFromTarget <= combat.BasicAttackSkill.MinRange && _curMobState.Equals(MobState.Pursue) && _isStrafer)
            {
                isStrafing = true;
            }
        }
        #endregion

        #region Knockback Methods
        /// <summary>
        /// Invokes knock back action
        /// </summary>
        public void ApplyKnockBack(Vector3 mobPos, KnockBackEffect effect)
        {
            if (combat.IsAttackLunging || IsUnstaggerable) return;

            combat.OnAttackInterrupt();
            isStrafingToPoint = false;
            Vector3 impactDir = (transform.position - mobPos).normalized;
            knockbackDir = new Vector3(impactDir.x, 0f, impactDir.z);
            activeKnockBackEffect = effect;
            effect.startPos = transform.position;
            anim.Play(StringData.Hit);
        }

        private void KnockbackHandler()
        {
            if (null != activeKnockBackEffect || _isStaggered)
            {
                _isStaggered = true;

                activeKnockBackEffect.maxKnockbackDuration -= Time.deltaTime;
                disFromStartPos = ParaverseHelper.GetDistance(activeKnockBackEffect.startPos, transform.position);

                // Ensures mob falls when off platform
                if (CheckFall())
                {
                    nav.enabled = false;
                    _isFalling = true;
                    knockbackDir.y = GlobalValues.GravityForce;
                    Vector3 fallDir = new Vector3(knockbackDir.x * activeKnockBackEffect.knockForce, knockbackDir.y * fallForce, knockbackDir.z * activeKnockBackEffect.knockForce);
                    controller.Move(fallDir * Time.deltaTime);

                    // Kills enemy within death timer upon fall
                    deathTimerUponFall += Time.deltaTime;
                    if (deathTimerUponFall >= maxDeathTimerUponFall)
                        stats.UpdateCurrentHealth(-stats.CurHealth);

                    return;
                }
                else if (disFromStartPos >= activeKnockBackEffect.maxKnockbackRange || activeKnockBackEffect.maxKnockbackDuration <= 0)
                {
                    CleanseStagger();
                    nav.enabled = true;
                    return;
                }

                // Moves the mob in the move direction
                controller.Move(knockbackDir * activeKnockBackEffect.knockForce * Time.deltaTime);
            }
        }
        #endregion

        #region Jump Methods
        public void ApplyJump(Vector3 mobPos)
        {
            if (_isJumping) return;

            curAirCheckTimer = jumpCheckTimer;
            nav.enabled = false;
            Debug.Log("Nav enabled on Apply Jump: " + nav.enabled);
            _isJumping = true;
            combat.OnAttackInterrupt();
            Vector3 targetDir = (mobPos - transform.position).normalized;
            landPos = mobPos;
            jumpDir = new Vector3(targetDir.x, targetDir.y, targetDir.z);
            jumpDir.y += Mathf.Sqrt(jumpHeightForce * -GlobalValues.GravityModifier * GlobalValues.GravityForce);
        }

        private void JumpHandler()
        {
            if (_isJumping)
            {
                Debug.Log("Nav enabled on Jump Handler: " + nav.enabled);
                Vector3 landDir = new Vector3(jumpDir.x * jumpLateralForce, jumpDir.y, jumpDir.z * jumpLateralForce);
                controller.Move(landDir * Time.deltaTime);
                //nav.updateRotation = false;

                if (curAirCheckTimer <= 0)
                {
                    if (IsGroundedCheck())
                    {
                        nav.enabled = true;
                        OnLandEvent?.Invoke();
                        _isJumping = false;
                        return;
                    }
                }
                else
                {
                    curAirCheckTimer -= Time.deltaTime;
                }

                jumpDir.y += GlobalValues.GravityForce * GlobalValues.GravityModifier * Time.deltaTime;
            }
        }

        /// <summary>
        /// Returns true if mob is grounded.
        /// </summary>
        /// <returns></returns>
        private bool IsGroundedCheck()
        {
            Vector3 origin = transform.position;
            Vector3 dir = -transform.up;
            
            if (Physics.Raycast(origin, dir * isGroundedCheckRange, out RaycastHit hit, isGroundedCheckRange, groundLayer))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Allows mob to fall off the map
        /// </summary>
        /// <returns></returns>
        private bool CheckFall()
        {
            Vector3 origin = transform.position;
            Vector3 dir = -transform.up;

            Vector3 topOrigin = origin + new Vector3(0f, 0f, nav.radius);
            Vector3 leftOrigin = origin + new Vector3(-nav.radius, 0f, 0f);
            Vector3 rightOrigin = origin + new Vector3(nav.radius, 0f, 0f);

            if (Physics.Raycast(topOrigin, dir * checkFallRange, checkFallRange) &&
            (Physics.Raycast(leftOrigin, dir * checkFallRange, checkFallRange) &&
            (Physics.Raycast(rightOrigin, dir * checkFallRange, checkFallRange))))
            {
                _isFalling = false;
            }
            else
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Fly Methods
        public void ApplyFlyUp(Vector3 mobPos)
        {
            if (isFlyingUp) return;

            curAirCheckTimer = jumpCheckTimer;
            nav.enabled = false;
            _isFlying = true;
            isFlyingUp = true;
            combat.OnAttackInterrupt();
            flyDir = transform.up;
            landPos = mobPos;
        }

        public void ApplyFlyDown(Vector3 mobPos)
        {
            if (isFlyingDown) return;

            nav.enabled = false;
            _isFlying = true;
            isFlyingDown = true;
            combat.OnAttackInterrupt();
            Vector3 targetDir = (mobPos - transform.position).normalized;
            landPos = mobPos;
            flyDir = new Vector3(targetDir.x, targetDir.y, targetDir.z);
        }

        private void FlyUpHandler()
        {
            if (_isFlying && isFlyingUp)
            {
                Vector3 dir = flyDir * flyUpForce;
                controller.Move(dir * Time.deltaTime);

                if (curAirCheckTimer <= 0)
                {
                    isFlyingUp = false;
                    return;
                }
                else
                {
                    curAirCheckTimer -= Time.deltaTime;
                }
                transform.rotation = ParaverseHelper.FaceTarget(transform, pursueTarget, rotSpeed);
            }
        }

        private void FlyDownHandler()
        {
            if (_isFlying && isFlyingDown)
            {
                Vector3 dir = flyDir * flyDownForce;
                controller.Move(dir * Time.deltaTime);

                if (IsGroundedCheck())
                {
                    nav.enabled = true;
                    _isFlying = false;
                    isFlyingUp = false;
                    isFlyingDown = false;
                    OnLandEvent?.Invoke();
                    return;
                }
                //flyDir.y += Mathf.Sqrt(flyDownForce * -GlobalValues.GravityModifier * GlobalValues.GravityForce);
            }
        }
        #endregion

        #region Hard and Soft CC Methods
        private void SoftCCHandler()
        {
            CleanseStagger();
        }

        private void HardCCHandler()
        {
            CleanseStagger();
        }
        #endregion

        #region Attack Movement
        private void AttackMovementHandler()
        {
            if (combat.IsAttackLunging)
            {
                nav.enabled = false;
                controller.Move(transform.forward * atkDashForce * Time.deltaTime);
            }
        }
        #endregion

        #region Status Effect Methods
        private void CleanseStagger()
        {
            if (CheckFall()) return;
            _isStaggered = false;
            activeKnockBackEffect = null;
        }
        #endregion

        #region Death Handler Methods
        private void DeathHandler()
        {
            if (stats.CurHealth <= 0 && _isDead == false)
            {
                _isDead = true;
                Death();
                OnDeathEvent?.Invoke(transform);
                Debug.Log("OnDeathEvent Invoked");
            }
        }

        private void Death()
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
            Destroy(gameObject.transform.parent.gameObject);
        }
        #endregion
    }
}
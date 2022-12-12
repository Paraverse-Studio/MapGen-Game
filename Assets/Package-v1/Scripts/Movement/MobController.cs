using Paraverse.Helper;
using Paraverse.Mob.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace Paraverse.Mob.Controller
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterController))]
    public class MobController : MonoBehaviour, IMobController
    {
        #region Variables
        // Important unity components
        private NavMeshAgent nav;
        private Animator anim;
        private CharacterController controller;
        // Reference to the combat script
        private IMobCombat combat;
        // Reference to the stats script
        IMobStats IMobController.Stats { get { return stats; } }
        private IMobStats stats;

        [Header("Movement Values"), Tooltip("The current speed of the mob")]
        private float curSpeed;
        [SerializeField, Range(0, 1), Tooltip("The walk speed of the mob.")]
        private float walkSpeedRatio = 1f;
        [SerializeField, Range(1, 10), Tooltip("The sprint speed of the mob.")]
        private float sprintSpeedRatio = 2.5f;
        [SerializeField, Tooltip("The rotation speed of the mob.")]
        private float rotSpeed = 100f;

        [Header("Patrol State Values")]
        [SerializeField, Tooltip("The waypoints the mob is going to patrol between.")]
        private Transform[] wps;
        private int curWPIdx;
        [SerializeField, Range(0, 10), Tooltip("The minimum wait duration for the mob at each waypoint.")]
        private float minWaitDur = 2f;
        [SerializeField, Range(0, 20), Tooltip("The maximum wait duration for the characther at each waypoint.")]
        private float maxWaitDur = 5f;
        [Tooltip("The wait duration for the mob, calculated from a random value between minWaitDuration and maxWaitDuration.")]
        private float startWaitDur;
        [Tooltip("The current wait timer, once it reaches the startWaitDuration, mob will proceed to next waypoint.")]
        private float curWaitTimer;
        [SerializeField, Tooltip("The distance at which the mob stops from its target waypoint.")]
        private float accurayWP = 2f;
        [SerializeField, Tooltip("The distance at which the mob stops from its current target.")]
        private float stoppingDistance = 2f;

        // Variables for detecting unreachable wps
        private Vector3 curPos;
        private Vector3 prevPos;
        private float unreachableWpCheckTimer = 0f;

        [Header("Pursue State Values")]
        [SerializeField, Tooltip("The range at which the mob can detect targets.")]
        private float detectionRadius = 5f;
        [SerializeField, Tooltip("The range at which the mob will stop pursuing target.")]
        private float pursueRadius = 7f;
        [Tooltip("The tag of the target that the mob can detect and pursue.")]
        private string targetTag = StringData.PlayerTag;
        [Tooltip("The targets transform")]
        private Transform pursueTarget;
        private IMobController playerController;
        [Tooltip("Set to true once target is detected.")]
        private bool targetDetected = false;

        [Header("Attack Dash Values")]
        [SerializeField, Tooltip("The attack dashing force applied during basic attack.")]
        private float atkDashForce = 2f;

        [Header("Knockback Values")]
        [SerializeField, Tooltip("The dive force of the mob.")]
        private float knockForce = 5f;
        [SerializeField, Range(0, 3), Tooltip("The max distance of dive.")]
        private float maxKnockbackRange = 1.5f;
        [SerializeField, Range(0, 1), Tooltip("The max duration of dive.")]
        private float maxKnockbackDuration = 1f;
        private float curKnockbackDuration;
        private Vector3 knockbackDir;
        private Vector3 knockStartPos;

        [Header("Fall Check")]
        [SerializeField, Tooltip("Checks if mob should fall off map.")]
        private float checkFallRange = 2f;
        [SerializeField, Tooltip("Timer for enemy death when falling off map.")]
        private float maxDeathTimerUponFall = 2f;
        private float deathTimerUponFall;

        [Header("Death Values")]
        [SerializeField]
        private GameObject deathEffect;
        public delegate void OnDeathDel(Transform target);
        public event OnDeathDel OnDeathEvent;

        // Reference to mob state
        [SerializeField, Tooltip("The current state of the mob.")]
        private MobState curState;
        public MobState CurMobState { get { return curState; } }

        // State Booleans 
        public Transform Transform { get { return transform; } }
        public bool IsInteracting { get { return _isInteracting; } }
        private bool _isInteracting = false;
        public bool IsKnockedBack { get { return _isKnockedBack; } }
        private bool _isKnockedBack = false;
        public bool IsDead { get { return _isDead; } }
        private bool _isDead = false;

        public IMobController Target { get { return _target; } }
        private IMobController _target;
        #endregion

        #region Start & Update Methods
        private void Start()
        {
            if (nav == null) nav = GetComponent<NavMeshAgent>();
            if (anim == null) anim = GetComponent<Animator>();
            if (controller == null) controller = GetComponent<CharacterController>();
            if (combat == null) combat = GetComponent<IMobCombat>();
            if (stats == null) stats = GetComponent<IMobStats>();

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
                Debug.LogError("Mob has no waypoint references!");
                return;
            }

            Initialize();
        }

        private void Update()
        {
            _isInteracting = anim.GetBool(StringData.IsInteracting);

            DeathHandler();
            if (_isDead) return;

            StateHandler();
            AnimatorHandler();
        }
        #endregion

        #region State Methods
        /// <summary>
        /// Responsible for handling and managing mob states.
        /// </summary>
        private void StateHandler()
        {
            nav.speed = curSpeed;
            if (_isInteracting && _isKnockedBack == false)
            {
                if (combat.IsAttackLunging)
                    AttackMovementHandler();
                else
                    nav.enabled = true;

                curSpeed = 0f;
                return;
            }

            if (_isKnockedBack)
            {
                curSpeed = 0f;
                KnockbackHandler();
                return;
            }

            if (TargetDetected() && combat.CanBasicAtk == false && playerController.IsDead == false)
            {
                PursueTarget();
                SetState(MobState.Pursue);
                //Debug.Log("Pursue State");
            }
            else if (TargetDetected() && combat.CanBasicAtk && playerController.IsDead == false)
            {
                CombatHandler();
                SetState(MobState.Combat);
                //Debug.Log("Combat State");
            }
            else
            {
                nav.updateRotation = true;
                Patrol();
                SetState(MobState.Patrol);
                //Debug.Log("Patrol State");
            }
        }

        /// <summary>
        /// Responsible for handling mob animation.
        /// </summary>
        private void AnimatorHandler()
        {
            anim.SetFloat(StringData.Speed, curSpeed);
            if (curState.Equals(MobState.Pursue) && curSpeed != 0)
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
        private void SetState(MobState state)
        {
            curState = state;
        }

        /// <summary>
        /// Returns the calculated value from patrolSpeedRatio and movement Speed stat.
        /// </summary>
        /// <returns></returns>
        private float GetPatrolSpeed()
        {
            return walkSpeedRatio * stats.MoveSpeed;
        }

        /// <summary>
        /// Returns the calculated value from pursueSpeedRatio and movement Speed stat.
        /// </summary>
        /// <returns></returns>
        private float GetPursueSpeed()
        {
            return sprintSpeedRatio * stats.MoveSpeed;
        }
        #endregion

        #region Patrolling Logic
        /// <summary>
        /// Responsible for mob patrolling. 
        /// </summary>
        private void Patrol()
        {
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
                    curSpeed = 0f;
                    nav.isStopped = true;
                    curWaitTimer += Time.deltaTime;
                }
                unreachableWpCheckTimer = 0f;
            }
            else
            {
                curSpeed = GetPatrolSpeed();
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
                Debug.Log(transform.name + "'s wp is out of reach! Updated out of reach wp to current position: " + transform.position);
            }
            prevPos = curPos;
        }

        /// <summary>
        /// Sets the next waypoint as the destination and resets the current waypoint timer.
        /// </summary>
        private void SetRandomWaypoint()
        {
            curWaitTimer = 0f;
            nav.isStopped = false;
            curSpeed = GetPatrolSpeed();
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
                Vector3 mobPos = ParaverseHelper.GetPositionXZ(transform.position);
                Vector3 targetPos = ParaverseHelper.GetPositionXZ(pursueTarget.position);
                float distanceFromTarget = ParaverseHelper.GetDistance(mobPos, targetPos);

                if (distanceFromTarget <= combat.BasicAtkRange)
                {
                    nav.isStopped = true;
                    curSpeed = 0f;
                    transform.rotation = ParaverseHelper.FaceTarget(transform, pursueTarget, rotSpeed);
                    //Debug.Log("Combat Idle");
                }
                else
                {
                    nav.updateRotation = true;
                    nav.isStopped = false;
                    curSpeed = GetPursueSpeed();
                    //Debug.Log("Pursuing");
                }
                nav.SetDestination(pursueTarget.position);
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
            if (pursueTarget == null)
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
            curSpeed = 0;
            nav.isStopped = true;
            transform.rotation = ParaverseHelper.FaceTarget(transform, pursueTarget, rotSpeed);

            // Ensures mob is looking at the target before attacking
            Vector3 dir = (pursueTarget.position - transform.position).normalized;
            dir = ParaverseHelper.GetPositionXZ(dir);
            Quaternion lookRot = Quaternion.LookRotation(dir);
            float angle = Quaternion.Angle(transform.rotation, lookRot);

            if (angle <= 0)
                combat.BasicAttackHandler();

            Debug.Log("Combat Handling");
        }
        #endregion

        #region Knockback Methods
        /// <summary>
        /// Invokes knock back action
        /// </summary>
        public void ApplyKnockBack(Vector3 mobPos)
        {
            Vector3 impactDir = (transform.position - mobPos).normalized;
            knockStartPos = transform.position;
            curKnockbackDuration = 0f;
            knockbackDir = new Vector3(impactDir.x, 0f, impactDir.z);
            _isKnockedBack = true;
            nav.enabled = false;
            if (_isInteracting == false)
                anim.Play(StringData.Hit);
        }

        /// <summary>
        /// Handles knock back movement and variables in Update().
        /// </summary>
        private void KnockbackHandler()
        {
            if (_isKnockedBack)
            {
                // Updates mob position and dive timer
                float knockBackRange = ParaverseHelper.GetDistance(transform.position, knockStartPos);
                curKnockbackDuration += Time.deltaTime;

                if (CheckFall())
                {
                    nav.enabled = false;
                    knockbackDir.y += GlobalValues.GravityForce * GlobalValues.GravityModifier;
                    controller.Move(knockbackDir * knockForce * Time.deltaTime);

                    // Kills enemy within death timer upon fall
                    deathTimerUponFall += Time.deltaTime;
                    if (deathTimerUponFall >= maxDeathTimerUponFall)
                        stats.UpdateCurrentHealth(-1000000);

                    return;
                }
                controller.Move(knockbackDir * knockForce * Time.deltaTime);

                // Stops dive when conditions met
                if (knockBackRange >= maxKnockbackRange || curKnockbackDuration >= maxKnockbackDuration)
                {
                    _isKnockedBack = false;
                    nav.enabled = true;
                }
            }
        }

        private bool CheckFall()
        {
            Vector3 origin = transform.position;
            Vector3 dir = -transform.up;

            if (Physics.Raycast(origin, dir * checkFallRange, checkFallRange))
            {
            }
            else
            {
                return true;
            }
            return false;
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

        #region Death Handler Methods
        private void DeathHandler()
        {
            if (stats.CurHealth <= 0 && _isDead == false)
            {
                _isDead = true;
                Death();
                OnDeathEvent?.Invoke(transform);
            }
        }

        private void Death()
        {
            Debug.Log("Player has died!");
            Instantiate(deathEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        #endregion
    }
}
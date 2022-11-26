using UnityEngine;
using UnityEngine.AI;

namespace Paraverse.Enemy.NavMesh
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class EnemyController : MonoBehaviour
    {
        // important unity components
        private NavMeshAgent nav;
        private Animator anim;
        private CharacterController controller;

        [Header("Important GameObjects References")]
        [SerializeField, Tooltip("The character head gameobject required for target detection.")]
        private Transform head;


        [SerializeField, Tooltip("The current state of the character.")]
        private EnemyState curState;
        public EnemyState CurEnemyState { get { return curState; } }

        [Header("Movement Values"), Tooltip("The current speed of the character")]
        private float curSpeed;
        [SerializeField, Range(0, 10), Tooltip("The patrolling speed of the character.")]
        private float patrolSpeed = 1.5f;
        [SerializeField, Range(0, 10), Tooltip("The sprint speed of the character.")]
        private float sprintSpeed = 3f;

        [Header("Patrol State Values")]
        [SerializeField, Tooltip("The waypoints the character is going to patrol between.")]
        private Transform[] waypoints;
        private int curWaypointIdx;
        [SerializeField, Range(0, 10), Tooltip("The minimum wait duration for the character at each waypoint.")]
        private float minWaitDuration = 5f;
        [SerializeField, Range(0, 20), Tooltip("The maximum wait duration for the characther at each waypoint.")]
        private float maxWaitDuration = 10f;
        [Tooltip("The wait duration for the character, calculated from a random value between minWaitDuration and maxWaitDuration.")]
        private float startWaitDuration;
        [Tooltip("The current wait timer, once it reaches the startWaitDuration, character will proceed to next waypoint.")]
        private float curWaitTimer;
        [SerializeField, Tooltip("The distance at which the character stops from its current target destination.")]
        private float stoppingDistance = 2f;

        [Header("Pursue State Values")]
        [SerializeField, Range(0, 30), Tooltip("The range at which the character can detect targets.")]
        private float detectionRange = 20f;
        [SerializeField, Tooltip("The tag of the target that the character can detect and pursue.")]
        private string targetTag = "Player";
        [Tooltip("The targets transform")]
        private Transform pursueTarget;
        [Tooltip("Set to true once target is detected.")]
        private bool targetDetected = false;

        [Header("Only for Raycast Detection")]
        [SerializeField, Tooltip("Sets characters detection method via raycast.")]
        private bool useRaycastDetection = false;
        [SerializeField, Range(0, 360), Tooltip("The angle at which the character can detect targets. (ONLY for useRaycastDetection=true)")]
        private float detectionAngle = 90f;
        [SerializeField, Tooltip("The character can detect this layer.(ONLY for useRaycastDetection=true)")]
        private LayerMask targetLayer;

        // Reference to the combat script
        private ICombat combat;



        private void Start()
        {
            if (nav == null) nav = GetComponent<NavMeshAgent>();
            if (anim == null) anim = GetComponent<Animator>();
            if (controller == null) controller = GetComponent<CharacterController>();
            if (combat == null) combat = GetComponent<BasicCombat>();

            if (combat.BasicAttackRange < stoppingDistance)
            {
                stoppingDistance = combat.BasicAttackRange;
                Debug.LogWarning(transform.name + " cannot have basic attack range lower than its stopping distance.");
            }
            Initialize();
        }

        private void Update()
        {
            StateHandler();
            AnimatorHandler();
        }

        #region Update Methods
        /// <summary>
        /// Responsible for handling and managing character states.
        /// </summary>
        private void StateHandler()
        {
            if (combat.IsInteracting)
            {
                curSpeed = 0f;
                return;
            }

            nav.speed = curSpeed;

            if (TargetDetected() && combat.CanBasicAttack == false)
            {
                PursueTarget();
                SetState(EnemyState.Pursue);
                Debug.Log("Pursue State");
            }
            else if (TargetDetected() && combat.CanBasicAttack)
            {
                CombatHandler();
                SetState(EnemyState.Combat);
                Debug.Log("Combat State");
            }
            else
            {
                Patrol();
                SetState(EnemyState.Patrol);
                Debug.Log("Patrol State");
            }
        }

        /// <summary>
        /// Responsible for handling character animation.
        /// </summary>
        private void AnimatorHandler()
        {
            anim.SetFloat("speed", curSpeed);
            if (curState.Equals(EnemyState.Pursue) && curSpeed != 0)
                anim.SetBool("isSprinting", true);
            else
                anim.SetBool("isSprinting", false);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Initializes the character upon Start().
        /// </summary>
        private void Initialize()
        {
            SetRandomWaypoint();
            nav.stoppingDistance = stoppingDistance;
            nav.acceleration = 999f;
            nav.angularSpeed = 999f;

            // Change this code if game is multiplayer
            if (useRaycastDetection == false)
            {
                pursueTarget = GameObject.FindGameObjectWithTag(targetTag).GetComponent<Transform>();
            }
            else
            {
                targetLayer = LayerMask.GetMask("Player");
            }
        }
        /// <summary>
        /// Only set state in StateHandler method to avoid bugs related to player's current state.
        /// </summary>
        /// <param name="state"></param>
        private void SetState(EnemyState state)
        {
            curState = state;
        }
        #endregion

        #region Patrolling Logic

        /// <summary>
        /// Responsible for character patrolling. 
        /// </summary>
        private void Patrol()
        {
            float distanceFromWaypoint = Vector3.Distance(transform.position, waypoints[curWaypointIdx].position);

            if (distanceFromWaypoint <= stoppingDistance)
            {
                nav.isStopped = true;
                if (curWaitTimer >= startWaitDuration)
                {
                    SetRandomWaypoint();
                }
                else
                {
                    curSpeed = 0f;
                    nav.isStopped = true;
                    curWaitTimer += Time.deltaTime;
                }
            }
            else
            {
                curSpeed = patrolSpeed;
                nav.isStopped = false;
            }
            nav.SetDestination(waypoints[curWaypointIdx].position);
        }

        /// <summary>
        /// Sets the next waypoint as the destination and resets the current waypoint timer.
        /// </summary>
        private void SetRandomWaypoint()
        {
            curWaitTimer = 0f;
            nav.isStopped = false;
            curSpeed = patrolSpeed;
            startWaitDuration = Random.Range(minWaitDuration, maxWaitDuration);
            int prevVal = curWaypointIdx;
            // Prevents character from going to current waypoint
            while (prevVal == curWaypointIdx)
            {
                curWaypointIdx = Random.Range(0, waypoints.Length - 1);
            }
            nav.SetDestination(waypoints[curWaypointIdx].position);
        }

        #endregion 

        #region Pursue Target Logic
        /// <summary>
        /// Responsible for handling character pursuing logic.
        /// </summary>
        private void PursueTarget()
        {
            if (pursueTarget != null)
            {
                float distanceFromTarget = Vector3.Distance(transform.position, pursueTarget.position);

                Debug.Log("Distance from target: " + distanceFromTarget + " position: "+pursueTarget.position);

                if (distanceFromTarget <= stoppingDistance)
                {
                    nav.isStopped = true;
                    curSpeed = 0f;
                    Debug.Log("Combat Idle");
                }
                else
                {
                    nav.isStopped = false;
                    curSpeed = sprintSpeed;
                    Debug.Log("Pursuing");
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
            if (useRaycastDetection)
            {
                return RaycastDetection();
            }
            else
            {
                return WithinDetectionRange();
            }
        }

        /// <summary>
        /// Simple detection method which checks whether target is within detection range. 
        /// Continues returning true until the character and target distance is double the detection range.
        /// </summary>
        /// <returns></returns>
        private bool WithinDetectionRange()
        {
            if (pursueTarget == null)
            {
                Debug.LogError("Pursue target cannot be null in simple detection method.");
                return false;
            }

            float distanceFromTarget = Vector3.Distance(transform.position, pursueTarget.position);

            if (distanceFromTarget <= detectionRange)
            {
                targetDetected = true;
                return true;
            }
            else if (targetDetected && distanceFromTarget <= detectionRange * 2f)
            {
                return true;
            }
            else
            {
                targetDetected = false;
                return false;
            }
        }

        /// <summary>
        /// Returns true if target is detected via Raycast.
        /// </summary>
        /// <returns></returns>
        private bool RaycastDetection()
        {
            Vector3 origin = transform.position;
            Vector3 dir = transform.forward * detectionRange;
            RaycastHit hit;

            Debug.DrawRay(origin, dir, Color.red);
            if (Physics.Raycast(origin, dir, out hit, detectionRange, targetLayer))
            {
                if (hit.collider.CompareTag(targetTag))
                {
                    nav.SetDestination(hit.collider.transform.position);
                    targetDetected = true;
                    Debug.Log("Detected Player: " + hit.collider.name);
                    return true;
                }
                return false;
            }
            return false;
        }

        #endregion

        #region Combat Logic
        /// <summary>
        /// Responsible for handling character combat.
        /// </summary>
        private void CombatHandler()
        {
            curSpeed = 0;
            nav.isStopped = true;
            combat.BasicAttackHandler();
        }

        #endregion
    }
}
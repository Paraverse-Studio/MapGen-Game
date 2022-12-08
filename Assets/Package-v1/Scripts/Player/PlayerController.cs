using Paraverse.Helper;
using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using UnityEngine;

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
        private Camera cam;
        private Animator anim;
        private CharacterController controller;
        private PlayerInputControls input;
        // Reference to the combat script
        private IMobCombat combat;
        // Reference to the stats script
        IMobStats IMobController.Stats { get { return stats; } }
        private IMobStats stats;

        [Header("Movement Values"), Tooltip("The current speed of the mob")]
        private float curSpeed;
        [SerializeField, Tooltip("The rotation speed of the mob.")]
        private float rotSpeed = 10f;

        [Header("Jump Values")]
        [SerializeField, Tooltip("The jump force of the mob.")]
        private float jumpForce = 10f;
        [SerializeField, Range(0.1f, 1f), Tooltip("Raycast distance to detect is Grounded")]
        private float disToGroundCheck = 0.1f;
        [SerializeField, Range(0, 2), Tooltip("Time required to wait in between each jump.")]
        private float jumpCd = 0.5f;
        private float curJumpCd = 0f;

        [Header("Gravity Values")]
        [SerializeField, Tooltip("The gravity force of the mob.")]
        private float gravityForce = -20f;
        [SerializeField, Tooltip("The gravity multiplier force.")]
        private float gravityMultiplier = 1f;

        [Header("Dive Values")]
        [SerializeField, Tooltip("The dive force of the mob.")]
        private float diveForce = 10f;
        [SerializeField, Range(0, 10), Tooltip("The max distance of dive.")]
        private float maxDiveRange = 3f;
        [SerializeField, Range(0, 3), Tooltip("The max duration of dive.")]
        private float maxDiveDuration = 1f;
        private float curDiveDuration;
        [SerializeField, Range(0, 2), Tooltip("Time required to wait in between each dive.")]
        private float diveCd = 0.5f;
        private float curDiveCd = 0f;

        [Header("Knockback Values")]
        [SerializeField, Tooltip("The dive force of the mob.")]
        private float knockForce = 5f;
        [SerializeField, Range(0, 3), Tooltip("The max distance of dive.")]
        private float maxKnockbackRange = 1.5f;
        [SerializeField, Range(0, 1), Tooltip("The max duration of dive.")]
        private float maxKnockbackDuration = 1f;
        private float curKnockbackDuration;

        // State Booleans
        public bool IsInteracting { get { return _isInteracting; } }
        private bool _isInteracting = false;
        public bool IsMoving { get { return _isMoving; } }
        private bool _isMoving = false;
        public bool IsGrounded { get { return _isGrounded; } }
        private bool _isGrounded = false;
        public bool IsDiving { get { return _isDiving; } }
        private bool _isDiving = false;
        public bool IsKnockedBack { get { return _isKnockedBack; } }
        private bool _isKnockedBack = false;

        // Movement, Jump & Dive inputs and velocities
        private Vector3 goalDir;
        private Vector3 moveDir;
        private Vector3 jumpDir;
        private Vector3 diveDir;
        private Vector3 knockbackDir;
        // Gets the start positions
        private Vector3 diveStartPos;
        private Vector3 knockStartPos;
        // Gets the controller horizontal and vertical inputs
        private float horizontal;
        private float vertical;
        #endregion

        #region Start & Update Methods
        private void Start()
        {
            if (cam == null) cam = Camera.main;
            if (anim == null) anim = GetComponentInChildren<Animator>();
            if (controller == null) controller = GetComponent<CharacterController>();
            if (input == null) input = GetComponent<PlayerInputControls>();
            if (combat == null) combat = GetComponent<IMobCombat>();
            if (stats == null) stats = GetComponent<IMobStats>();

            // Subscribes code to mob death event listener
            GameObject[] mobs = GameObject.FindGameObjectsWithTag(StringData.EnemyTag);
            for (int i = 0; i < mobs.Length; i++)
            {
                mobs[i].GetComponent<MobController>().OnDeathEvent += OnKillTest;
            }

            // Subscribes item pick up code to use item event listener
            input.OnUseItemOneEvent += UseItemOne;
            input.OnUseItemTwoEvent += UseItemTwo;
            input.OnUseItemThreeEvent += UseItemThree;
            input.OnUseItemFourEvent += UseItemFour;
            input.OnJumpEvent += Jump;
            input.OnDiveEvent += Dive;
            input.OnTargetLockEvent += TargetLock;
        }

        private void Update()
        {
            _isInteracting = anim.GetBool(StringData.IsInteracting);
            _isMoving = Mathf.Abs(vertical) > 0 || Mathf.Abs(horizontal) > 0;
            _isGrounded = IsGroundedCheck();

            jumpDir.y -= Time.deltaTime;

            MovementHandler();
            RotationHandler();
            JumpHandler();
            DiveHandler();
            KnockbackHandler();
            AnimationHandler();
        }
        #endregion

        #region Helper Methods
        private float GetWalkSpeed()
        {
            return stats.MoveSpeed;
        }
        #endregion

        #region Movement Handler Methods
        private void AnimationHandler()
        {
            anim.SetFloat(StringData.Speed, moveDir.magnitude);
            anim.SetBool(StringData.IsGrounded, IsGrounded);
            anim.SetBool(StringData.IsDiving, IsDiving);
            anim.SetBool(StringData.IsKnockedBack, IsKnockedBack);
        }

        private void MovementHandler()
        {
            // Disables player movement during dive
            if (_isDiving || _isKnockedBack || _isInteracting) return;

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

        private void RotationHandler()
        {
            if (_isKnockedBack) return;

            // Ensures player faces the direction in which they were moving when stopped
            //if (goalDir != Vector3.zero)
            //{
            //    transform.forward = moveDir;
            //}
            
            if (moveDir != Vector3.zero)
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
        private void Jump()
        {
            if (_isKnockedBack || _isInteracting) return;

            if (_isGrounded && curJumpCd >= jumpCd)
            {
                curJumpCd = 0f;
                jumpDir.y += Mathf.Sqrt(jumpForce * -gravityMultiplier * gravityForce);
                anim.Play(StringData.Jump);
            }
        }

        /// <summary>
        /// Handles jump movement and variables in Update().
        /// </summary>
        private void JumpHandler()
        {
            ApplyGravity();

            curJumpCd += Time.deltaTime;
            curJumpCd = Mathf.Clamp(curJumpCd, 0, jumpCd);

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
            jumpDir.y += gravityForce * Time.deltaTime;
        }

        /// <summary>
        /// Returns true if player is grounded.
        /// </summary>
        /// <returns></returns>
        private bool IsGroundedCheck()
        {
            Vector3 origin = transform.position;
            Vector3 dir = -transform.up;

            Debug.DrawRay(origin, dir, Color.red);
            if (Physics.Raycast(origin, dir * disToGroundCheck, disToGroundCheck))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Dive Handler
        /// <summary>
        /// Invokes dive action
        /// </summary>
        private void Dive()
        {
            if (_isKnockedBack || _isDiving || _isInteracting || _isMoving == false) return;

            if (_isGrounded && curDiveCd >= diveCd)
            {
                stats.ConsumeDiveEnergy();
                _isDiving = true;
                curDiveCd = 0f;
                curDiveDuration = 0f;
                diveStartPos = transform.position;
                controller.detectCollisions = false;
                diveDir = new Vector3(goalDir.x, 0f, goalDir.z);
                anim.Play(StringData.Dive);
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
                    return;
                }
            }

            curDiveCd += Time.deltaTime;
            curDiveCd = Mathf.Clamp(curDiveCd, 0, diveCd);
        }

        private void TargetLock()
        {
            TargetLockSystem.Instance.ToggleSelect();
        }

        #endregion

        #region KnockBack Handler
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

                // Moves the mob in the move direction
                controller.Move(knockbackDir * knockForce * Time.deltaTime);

                // Stops dive when conditions met
                if (knockBackRange >= maxKnockbackRange || curKnockbackDuration >= maxKnockbackDuration)
                {
                    _isKnockedBack = false;
                    return;
                }
            }
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

        private void OnKillTest(Transform target)
        {
            Debug.Log(transform.name + " killed " + target.name);
        }
    }
}
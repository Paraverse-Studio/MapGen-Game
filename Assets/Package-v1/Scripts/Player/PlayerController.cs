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
        [SerializeField, Range(0, 3), Tooltip("The max distance of dive.")]
        private float maxDiveRange = 3f;
        [SerializeField, Range(0, 1), Tooltip("The max duration of dive.")]
        private float maxDiveDuration = 1f;
        private float curDiveDuration;
        [SerializeField, Range(0, 2), Tooltip("Time required to wait in between each dive.")]
        private float diveCd = 0.5f;
        private float curDiveCd = 0f;

        // State Booleans
        public bool IsInteracting { get { return isInteracting; } }
        private bool isInteracting = false;
        private bool isSprinting = false;
        public bool IsMoving { get { return _isMoving; } }
        private bool _isMoving = false;
        public bool IsGrounded { get { return _isGrounded; } }
        private bool _isGrounded = false;
        public bool IsDiving { get { return _isDiving; } }
        private bool _isDiving = false;

        // Movement, Jump & Dive inputs and velocities
        private Vector3 moveDir;
        private Vector3 jumpDir;
        private Vector3 diveDir;
        // Gets the dive start position
        private Vector3 diveStartPos;
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
        }

        private void Update()
        {
            isInteracting = anim.GetBool(StringData.IsInteracting);
            isSprinting = input.IsSprinting;
            _isMoving = moveDir.magnitude > 0;
            Debug.Log("Is Moving: " + _isMoving);
            _isGrounded = IsGroundedCheck();

            jumpDir.y -= Time.deltaTime;

            MovementHandler();
            JumpHandler();
            DiveHandler();
            RotationHandler();
            AnimationHandler();
        }
        #endregion

        #region Controller Interface Methods
        public void ApplyHitAnimation()
        {
            Debug.Log("hit animation: " + IsInteracting);
            if (IsInteracting == false)
                anim.Play(StringData.Hit);
        }
        #endregion

        #region Movement Handler Methods
        private void AnimationHandler()
        {
            anim.SetFloat(StringData.Speed, moveDir.normalized.magnitude);
            anim.SetBool(StringData.IsSprinting, isSprinting);
            anim.SetBool(StringData.IsGrounded, IsGrounded);
            anim.SetBool(StringData.IsDiving, IsDiving);
        }

        private void MovementHandler()
        {
            // Adjusts player speed based on state
            if (IsInteracting)
                curSpeed = 0f;
            else
                curSpeed = GetWalkSpeed();

            // Disables player movement during dive
            if (_isDiving) return;

            // Gets movement input values
            horizontal = input.MovementDirection.x;
            vertical = input.MovementDirection.y;

            // Makes player movement relative to camera
            moveDir = new Vector3(horizontal, jumpDir.y, vertical);
            moveDir = moveDir.x * new Vector3(cam.transform.right.x, 0, cam.transform.right.z) + moveDir.z * new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
            moveDir.Normalize();

            controller.Move(moveDir * curSpeed * Time.deltaTime);
        }

        private void RotationHandler()
        {
            // Ensures player faces the direction in which they were moving when stopped
            if (moveDir != Vector3.zero)
                transform.forward = moveDir;
        }
        #endregion

        #region Jump Handler Methods
        /// <summary>
        /// Invokes jump action
        /// </summary>
        private void Jump()
        {
            if (controller.isGrounded && curJumpCd >= jumpCd)
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
            if (jumpDir.y < 0 && IsGrounded)
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
            if (controller.isGrounded && curDiveCd >= diveCd && _isDiving == false && _isMoving)
            {
                diveStartPos = transform.position;
                curDiveDuration = 0f;
                curDiveCd = 0f;
                diveDir = new Vector3(moveDir.x, jumpDir.y, moveDir.z);
                _isDiving = true;
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
                    _isDiving = false;
                    return;
                }
            }

            curDiveCd += Time.deltaTime;
            curDiveCd = Mathf.Clamp(curDiveCd, 0, diveCd);
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
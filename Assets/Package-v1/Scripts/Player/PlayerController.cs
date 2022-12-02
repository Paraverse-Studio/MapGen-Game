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
        [SerializeField, Tooltip("The walk speed of the mob.")]
        private float walkSpeedRatio = 5f;
        [SerializeField, Tooltip("The sprint speed of the mob.")]
        private float sprintSpeedRatio = 7f;
        [SerializeField, Tooltip("The rotation speed of the mob.")]
        private float rotSpeed = 10f;

        [Header("Jump Values")]
        [SerializeField, Tooltip("The rotation speed of the mob.")]
        private float jumpForce = 10f;
        [SerializeField, Tooltip("The rotation speed of the mob.")]
        private float gravityValue = -20f;
        [SerializeField, Tooltip("The rotation speed of the mob.")]
        private float gravityMultiplier = 1f;
        [SerializeField, Tooltip("Raycast distance to detect is Grounded")]
        private float disToGroundCheck = 0.1f;
        //[SerializeField, Tooltip("Ground detection layer")]
        //private float groundLayer = StringData.Default;

        // State Booleans
        public bool IsInteracting { get { return isInteracting; } }
        private bool isInteracting = false;
        private bool isSprinting = false;
        public bool IsGrounded { get { return _isGrounded; } }
        private bool _isGrounded = false;

        // Movement & Jump inputs and velocities
        private Vector3 moveDir;
        private Vector3 jumpVelocity;
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
        }

        private void Update()
        {
            isInteracting = anim.GetBool(StringData.IsInteracting);
            isSprinting = input.IsSprinting;
            _isGrounded = IsGroundedCheck();
            
            MovementHandler();
            JumpHandler();
            RotationHandler();
            AnimationHandler();
        }
        #endregion

        #region Helper Methods
        private float GetWalkSpeed()
        {
            return walkSpeedRatio * stats.MoveSpeed;
        }

        private float GetSprintSpeed()
        {
            return sprintSpeedRatio * stats.MoveSpeed;
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
        }

        private void MovementHandler()
        {
            // Adjusts player speed based on state
            if (IsInteracting)
                curSpeed = 0f;
            else if (input.IsSprinting)
                curSpeed = GetSprintSpeed();
            else
                curSpeed = GetWalkSpeed();

            // Gets movement input values
            horizontal = input.MovementDirection.x;
            vertical = input.MovementDirection.y;

            // Makes player movement relative to camera
            moveDir = new Vector3(horizontal, jumpVelocity.y, vertical);
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
            if (controller.isGrounded)
            {
                jumpVelocity.y += Mathf.Sqrt(jumpForce * -gravityMultiplier * -gravityValue);
                anim.Play(StringData.Jump);
            }
        }
        

        private void JumpHandler()
        {
            ApplyGravity();

            controller.Move(jumpVelocity * Time.deltaTime);
        }

        private void ApplyGravity()
        {
            // Ensures player remains grounded when grounded
            if (jumpVelocity.y < 0 && IsGrounded)
            {
                jumpVelocity.y = 0f;
            }

            // Applies gravity and jump movement
            jumpVelocity.y += gravityValue * Time.deltaTime;
        }

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
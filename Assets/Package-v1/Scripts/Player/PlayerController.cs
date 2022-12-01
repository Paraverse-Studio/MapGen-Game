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
    public class PlayerController : MonoBehaviour
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

        // State Booleans
        private bool isSprinting = false;

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
        }

        private void Update()
        {
            isSprinting = input.IsSprinting;
            
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

        #region Movement Handler Methods
        private void AnimationHandler()
        {
            anim.SetFloat(StringData.Speed, moveDir.normalized.magnitude);
            anim.SetBool(StringData.IsSprinting, isSprinting);
        }

        private void MovementHandler()
        {
            // Adjusts player speed based on state
            if (anim.GetBool(StringData.IsInteracting))
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
        
        private void JumpHandler()
        {
            // Ensures player remains grounded when grounded
            if (jumpVelocity.y < 0 && controller.isGrounded)
            {
                jumpVelocity.y = 0f;
            }

            // Allows player jump 
            if (input.IsJumping && controller.isGrounded)
            {
                jumpVelocity.y += Mathf.Sqrt(jumpForce * -gravityMultiplier * -gravityValue);
            }

            // Applies gravity and jump movement
            jumpVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(jumpVelocity * Time.deltaTime);
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
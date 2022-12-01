using UnityEngine;
using UnityEngine.InputSystem;

namespace Paraverse.Player
{
    public class PlayerInputControls : MonoBehaviour
    {
        #region Variables
        private PlayerInputs input;
        private InputAction movement;
        private InputAction rotation;

        public delegate void OnJumpDel();
        public event OnJumpDel OnJumpEvent;
        public delegate void OnBasicAttackDel();
        public event OnBasicAttackDel OnBasicAttackEvent;
        public delegate void OnUseItemOneDel();
        public event OnBasicAttackDel OnUseItemOneEvent;
        public delegate void OnUseItemTwoDel();
        public event OnBasicAttackDel OnUseItemTwoEvent;
        public delegate void OnUseItemThreeDel();
        public event OnBasicAttackDel OnUseItemThreeEvent;
        public delegate void OnUseItemFourDel();
        public event OnUseItemFourDel OnUseItemFourEvent;
        public delegate void OnPauseDel();
        public event OnPauseDel OnPauseEvent;

        public Vector2 MovementDirection { get { return _movementDirection; } }
        private Vector2 _movementDirection;
        public Vector2 RotationDirection { get { return _rotationDirection; } }
        private Vector2 _rotationDirection;
        public bool IsSprinting { get { return _isSprinting; } }
        private bool _isSprinting = false;
        public bool IsJumping { get { return _isJumping; } }
        private bool _isJumping = false;
        #endregion

        #region Singleton
        public static PlayerInputControls Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            input = new PlayerInputs();
        }
        #endregion

        #region Enable & Disable Methods
        private void OnEnable()
        {
            movement = input.Player.Movement;
            rotation = input.Player.Rotation;

            input.Player.Sprint.performed += OnSprint;
            input.Player.Sprint.canceled += OffSprint;
            input.Player.Sprint.Enable();

            input.Player.Jump.performed += OnJump;
            input.Player.Jump.Enable();
            input.Player.Jump.canceled += OffJump;
            input.Player.Jump.Enable();

            input.Player.BasicAttack.performed += OnBasicAttack;
            input.Player.BasicAttack.Enable();

            input.Player.ItemOne.performed += OnUseItemOne;
            input.Player.ItemOne.Enable();
            input.Player.ItemTwo.performed += OnUseItemTwo;
            input.Player.ItemTwo.Enable();
            input.Player.ItemThree.performed += OnUseItemThree;
            input.Player.ItemThree.Enable();
            input.Player.ItemFour.performed += OnUseItemFour;
            input.Player.ItemFour.Enable();

            input.Player.Pause.performed += OnPause;
            input.Player.Pause.Enable();

            input.Enable();
        }

        private void OnDisable()
        {
            movement = input.Player.Movement;
            rotation = input.Player.Rotation;

            input.Player.Sprint.performed -= OnSprint;
            input.Player.Sprint.canceled -= OffSprint;
            input.Player.Sprint.Disable();

            input.Player.Jump.performed -= OnJump;
            input.Player.Jump.canceled -= OffJump;
            input.Player.Jump.Disable();

            input.Player.BasicAttack.performed -= OnBasicAttack;
            input.Player.BasicAttack.Disable();

            input.Player.ItemOne.performed -= OnUseItemOne;
            input.Player.ItemOne.Disable();
            input.Player.ItemTwo.performed -= OnUseItemTwo;
            input.Player.ItemTwo.Disable();
            input.Player.ItemThree.performed -= OnUseItemThree;
            input.Player.ItemThree.Disable();
            input.Player.ItemFour.performed -= OnUseItemFour;
            input.Player.ItemFour.Disable();

            input.Player.Pause.performed -= OnPause;
            input.Player.Pause.Disable();

            input.Disable();
        }
        #endregion

        #region Update Methods
        private void Update()
        {
            _movementDirection = movement.ReadValue<Vector2>();
            _rotationDirection = rotation.ReadValue<Vector2>();
        }
        #endregion

        #region Event Methods
        private void OnSprint(InputAction.CallbackContext obj)
        {
            _isSprinting = true;
        }

        private void OffSprint(InputAction.CallbackContext obj)
        {
            _isSprinting = false;
        }

        private void OnJump(InputAction.CallbackContext obj)
        {
            _isJumping = true;
        }

        private void OffJump(InputAction.CallbackContext obj)
        {
            _isJumping = false;
        }

        private void OnBasicAttack(InputAction.CallbackContext obj)
        {
            OnBasicAttackEvent?.Invoke();
        }

        private void OnUseItemOne(InputAction.CallbackContext obj)
        {
            OnUseItemOneEvent?.Invoke();
        }

        private void OnUseItemTwo(InputAction.CallbackContext obj)
        {
            OnUseItemTwoEvent?.Invoke();
        }

        private void OnUseItemThree(InputAction.CallbackContext obj)
        {
            OnUseItemThreeEvent?.Invoke();
        }

        private void OnUseItemFour(InputAction.CallbackContext obj)
        {
            OnUseItemFourEvent?.Invoke();
        }

        private void OnPause(InputAction.CallbackContext obj)
        {
            OnPauseEvent?.Invoke();
        }
        #endregion
    }
}
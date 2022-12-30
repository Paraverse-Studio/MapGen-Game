using UnityEngine;
using UnityEngine.InputSystem;

namespace Paraverse.Player
{
    public class PlayerInputControls : MonoBehaviour
    {
        #region Variables
        private PlayerInputs input;
        private InputAction movement;

        public delegate void OnJumpDel();
        public event OnJumpDel OnJumpEvent;
        public delegate void OnDiveDel();
        public event OnDiveDel OnDiveEvent;
        public delegate void OnTargetLockDel();
        public event OnTargetLockDel OnTargetLockEvent;
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
        public delegate void OnSkillUseDel();
        public event OnSkillUseDel OnSkillOneEvent;

        public Vector2 MovementDirection { get { return _movementDirection; } }
        private Vector2 _movementDirection;
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

            input.Player.BasicAttack.performed += OnBasicAttack;
            input.Player.BasicAttack.Enable();

            input.Player.Jump.performed += OnJump;
            input.Player.Dive.performed += OnDive;
            input.Player.TargetLock.performed += OnTargetLock;

            input.Player.ItemOne.performed += OnUseItemOne;
            input.Player.ItemOne.Enable();
            input.Player.ItemTwo.performed += OnUseItemTwo;
            input.Player.ItemTwo.Enable();
            input.Player.ItemThree.performed += OnUseItemThree;
            input.Player.ItemThree.Enable();
            input.Player.ItemFour.performed += OnUseItemFour;
            input.Player.ItemFour.Enable();

            input.Player.SkillOne.performed += OnSkillOne;
            input.Player.SkillOne.Enable();

            input.Player.Pause.performed += OnPause;
            input.Player.Pause.Enable();

            input.Enable();
        }

        private void OnDisable()
        {
            movement = input.Player.Movement;

            _movementDirection = Vector2.zero;
            input.Player.BasicAttack.performed -= OnBasicAttack;
            input.Player.BasicAttack.Disable();

            input.Player.Jump.performed -= OnJump;
            input.Player.Dive.performed -= OnDive;
            input.Player.TargetLock.performed -= OnTargetLock;

            input.Player.ItemOne.performed -= OnUseItemOne;
            input.Player.ItemOne.Disable();
            input.Player.ItemTwo.performed -= OnUseItemTwo;
            input.Player.ItemTwo.Disable();
            input.Player.ItemThree.performed -= OnUseItemThree;
            input.Player.ItemThree.Disable();
            input.Player.ItemFour.performed -= OnUseItemFour;
            input.Player.ItemFour.Disable();

            input.Player.SkillOne.performed -= OnSkillOne;
            input.Player.SkillOne.Enable();

            input.Player.Pause.performed -= OnPause;
            input.Player.Pause.Disable();

            input.Disable();
        }
        #endregion

        #region Update Methods
        private void Update()
        {
            _movementDirection = movement.ReadValue<Vector2>();
        }
        #endregion

        #region Event Methods
        private void OnJump(InputAction.CallbackContext obj)
        {
            OnJumpEvent?.Invoke();
        }

        private void OnDive(InputAction.CallbackContext obj)
        {
            OnDiveEvent?.Invoke();
        }

        private void OnTargetLock(InputAction.CallbackContext obj)
        {
            OnTargetLockEvent?.Invoke();
        }

        private void OnBasicAttack(InputAction.CallbackContext obj)
        {
            OnBasicAttackEvent?.Invoke();
        }

        private void OnSkillOne(InputAction.CallbackContext obj)
        {
            OnSkillOneEvent?.Invoke();
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
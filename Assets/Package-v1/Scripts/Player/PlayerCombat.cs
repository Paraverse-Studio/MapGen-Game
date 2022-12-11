using Paraverse.Mob.Combat;
using UnityEngine;

namespace Paraverse.Player
{
    public class PlayerCombat : MobCombat
    {
        private new PlayerController controller;
        private PlayerInputControls input;

        public int BasicAttackComboIdx { get { return basicAtkComboIdx; } }
        private int basicAtkComboIdx = 0;
        private int basicAtkComboIdxLimit = 2;
        [SerializeField, Tooltip("Max cooldown to allow next combo attack.")]
        private float maxComboResetTimer = 1f;
        private float curCombatResetTimer;

        protected override void Start()
        {
            base.Start();
            controller = gameObject.GetComponent<PlayerController>();
            input = GetComponent<PlayerInputControls>();    
            input.OnBasicAttackEvent += ApplyBasicAttack;
        }

        protected override void Update()
        {
            base.Update();
            BasicAttackComboHandler();
        }

        /// <summary>
        /// Runs on attack
        /// </summary>
        private void ApplyBasicAttack()
        {
            if (controller.IsAvoidLandingOn) return;

            if (controller.IsInteracting == false || anim.GetBool(StringData.CanBasicAttackTwo) || anim.GetBool(StringData.CanBasicAttackThree))
            {
                GetBasicAttackCombo();
            }
        }

        private void GetBasicAttackCombo()
        {
            Debug.Log("Index: " + basicAtkComboIdx + "      bool2: " + anim.GetBool(StringData.CanBasicAttackTwo) + 
                                                     "      bool3: " + anim.GetBool(StringData.CanBasicAttackThree));


            if (anim.GetBool(StringData.CanBasicAttackThree)) // && basicAtkComboIdx == 2)
            {
                Debug.Log("ATTK 3 called " + basicAtkComboIdx);
                basicAtkComboIdx++;
                anim.SetBool(StringData.IsInteracting, true);
                anim.Play(StringData.BasicAttackThree);
            }
            else if (anim.GetBool(StringData.CanBasicAttackTwo)) // && basicAtkComboIdx == 1)
            {
                Debug.Log("ATTK 2 called " + basicAtkComboIdx);

                basicAtkComboIdx++;
                anim.SetBool(StringData.IsInteracting, true);
                anim.Play(StringData.BasicAttackTwo);
            }
            else if (basicAtkComboIdx == 0)
            {
                Debug.Log("ATTK 1 called " + basicAtkComboIdx);

                basicAtkComboIdx++;
                anim.Play(StringData.BasicAttack);
            }
            
            ResetAnimationBasicAttackStates();

            // increment combo atk idx
            //basicAtkComboIdx++;
            curCombatResetTimer = maxComboResetTimer;
            if (basicAtkComboIdx > basicAtkComboIdxLimit)
            {
                basicAtkComboIdx = 0;
            }
        }

        private void ResetAnimationBasicAttackStates()
        {
            _isAttackLunging = false;
            anim.SetBool(StringData.CanBasicAttackTwo, false);
            anim.SetBool(StringData.CanBasicAttackThree, false);
        }

        private void BasicAttackComboHandler()
        {
            if (curCombatResetTimer <= 0)
            {
                ResetAnimationBasicAttackStates();
                basicAtkComboIdx = 0;
            }
            else
            {
                curCombatResetTimer -= Time.deltaTime;
            }
        }

        public void AddListenerOnBasicAttack()
        {
            input.OnBasicAttackEvent += BasicAttackHandler;
        }

        public void RemoveListenerOnBasicAttack()
        {
            input.OnBasicAttackEvent -= BasicAttackHandler;
        }

        #region Animation Events
        public void EnableBasicAttackTwo()
        {
            ResetAnimationBasicAttackStates();
            anim.SetBool(StringData.CanBasicAttackTwo, true);
        }

        public void EnableBasicAttackThree()
        {
            ResetAnimationBasicAttackStates();
            anim.SetBool(StringData.CanBasicAttackThree, true);
        }
        #endregion
    }
}

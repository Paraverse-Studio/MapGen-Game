using Paraverse.Mob.Combat;
using UnityEngine;

namespace Paraverse.Player
{
    public class PlayerCombat : MobCombat
    {
        private PlayerController controller;
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
        
        private void ApplyBasicAttack()
        {
            if (controller.IsAvoidLandingOn) return;

            if (controller.IsInteracting == false)
            {
                GetBasicAttackCombo();
                curBasicAtkCd = GetBasicAttackCooldown();
                //Debug.Log("Basic attack");
            }
        }

        public override void BasicAttackHandler()
        {
            BasicAttackComboHandler();
        }

        private void GetBasicAttackCombo()
        {
            switch (basicAtkComboIdx)
            {
                case 0:
                    anim.Play(StringData.BasicAttack);
                    break;
                case 1:
                    anim.Play(StringData.BasicAttackTwo);
                    break;
                case 2:
                    anim.Play(StringData.BasicAttackThree);
                    break;
            }

            // increment combo atk idx
            basicAtkComboIdx++;
            curCombatResetTimer = maxComboResetTimer;
            if (basicAtkComboIdx > basicAtkComboIdxLimit)
                basicAtkComboIdx = 0;
        }

        private void BasicAttackComboHandler()
        {
            if (curCombatResetTimer <= 0)
            {
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
    }
}

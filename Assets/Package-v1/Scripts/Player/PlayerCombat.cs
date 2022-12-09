using Paraverse.Mob.Combat;

namespace Paraverse.Player
{
    public class PlayerCombat : MobCombat
    {
        private PlayerController controller;
        private PlayerInputControls input;

        protected override void Start()
        {
            base.Start();
            controller = gameObject.GetComponent<PlayerController>();
            input = GetComponent<PlayerInputControls>();    
            input.OnBasicAttackEvent += BasicAttackHandler;
        }

        public override void BasicAttackHandler()
        {
            if (controller.IsAvoidLandingOn) return;

            base.BasicAttackHandler();
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

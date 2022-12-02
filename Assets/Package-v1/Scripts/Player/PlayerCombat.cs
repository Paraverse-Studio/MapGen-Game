using Paraverse.Mob.Combat;

namespace Paraverse.Player
{
    public class PlayerCombat : MobCombat
    {
        private PlayerInputControls input;

        protected override void Start()
        {
            base.Start();
            input = GetComponent<PlayerInputControls>();    
            input.OnBasicAttackEvent += BasicAttackHandler;
        }
    }
}

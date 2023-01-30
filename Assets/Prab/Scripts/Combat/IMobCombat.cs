namespace Paraverse.Mob 
{
    public interface IMobCombat
    {
        public bool IsBasicAttacking { get; }
        public bool IsAttackLunging { get; }
        public float BasicAtkRange { get; }
        public bool CanBasicAtk { get; }
        public bool IsSkilling { get; set; }
        public abstract void BasicAttackHandler();
        public abstract void OnAttackInterrupt();
    }
}
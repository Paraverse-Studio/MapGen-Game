namespace Paraverse.Mob
{
    public interface IMobCombat
    {
        public bool IsBasicAttacking { get; }
        public bool IsAttackLunging { get; }
        public BasicAttackSkill BasicAttackSkill { get; }
        public float BasicAtkRange { get; }
        public bool CanBasicAtk { get; }
        public bool IsUsingSkilling { get; set; }
        public bool IsInCombat { get; }
        public abstract void OnAttackInterrupt();
    }
}
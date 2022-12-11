namespace Paraverse.Mob 
{
    public interface IMobCombat
    {
        public bool IsAttackLunging { get; }
        public float BasicAtkRange { get; }
        public bool CanBasicAtk { get; }
        public abstract void BasicAttackHandler();
        public abstract void OnAttackInterrupt();
    }
}
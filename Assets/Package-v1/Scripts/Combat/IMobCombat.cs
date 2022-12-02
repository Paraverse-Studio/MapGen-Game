namespace Paraverse.Mob 
{
    public interface IMobCombat
    {
        public float BasicAtkRng { get; }
        public bool IsInteracting { get; }
        public bool CanBasicAtk { get; }
        public abstract void BasicAttackHandler();
    }
}
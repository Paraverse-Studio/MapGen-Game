namespace Paraverse.Mob 
{
    public interface IMobCombat
    {
        public float BasicAtkRange { get; }
        public bool CanBasicAtk { get; }
        public abstract void BasicAttackHandler();
    }
}
public interface ICombat
{
    public float BasicAttackRange { get; }
    public bool IsInteracting { get; }
    public bool CanBasicAttack { get; }
    public abstract void BasicAttackHandler();
}

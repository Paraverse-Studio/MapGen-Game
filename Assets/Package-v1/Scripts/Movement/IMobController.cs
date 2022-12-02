using Paraverse.Mob.Stats;

namespace Paraverse.Mob
{
    public interface IMobController
    {
        public IMobStats Stats { get; }
        public void ApplyHitAnimation();
    }
}
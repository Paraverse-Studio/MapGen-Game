using UnityEngine;
using Paraverse.Mob.Stats;

namespace Paraverse.Mob
{
    public interface IMobController
    {
        public bool IsKnockedBack { get; }
        public bool IsDead { get; }
        public IMobStats Stats { get; }
        public IMobController Target { get; }
        public void ApplyKnockBack(Vector3 mobPos);
    }
}
using UnityEngine;
using Paraverse.Mob.Stats;

namespace Paraverse.Mob
{
    public interface IMobController
    {
        public Transform Transform { get; }
        public bool IsInteracting { get; }
        public bool IsBasicAttacking { get; }
        public bool IsUsingSkill { get; }
        public bool IsKnockedBack { get; }
        public bool IsDead { get; }
        public IMobStats Stats { get; }
        public void ApplyKnockBack(Vector3 mobPos);

        public delegate void OnDeathDel(Transform target);
        public event OnDeathDel OnDeathEvent;
    }
}
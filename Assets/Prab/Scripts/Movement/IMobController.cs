using UnityEngine;
using Paraverse.Mob.Stats;

namespace Paraverse.Mob
{
  public interface IMobController
  {
    public Transform Transform { get; }
    public bool IsInteracting { get; }
    public bool IsStaggered { get; }
    public bool IsHardCCed { get; }
    public bool IsSoftCCed { get; }
    public bool IsUnstaggerable { get; }
    public bool IsDead { get; }
    public KnockBackEffect ActiveKnockBackEffect { get; }
    public IMobStats Stats { get; }
    public void ApplyHitAnimation();
    public void ApplyKnockBack(Vector3 mobPos, KnockBackEffect effect);

    public delegate void OnDeathDel(Transform target);
    public event OnDeathDel OnDeathEvent;
  }
}
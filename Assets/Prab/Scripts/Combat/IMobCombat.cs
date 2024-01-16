using Paraverse.Combat;
using Paraverse.Mob.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace Paraverse.Mob
{
  public interface IMobCombat
  {
    public Animator Anim { get; }
    public MobStats Stats { get; }
    public IMobController Controller { get; }
    public BasicAttackSkill BasicAttackSkill { get; }
    public float BasicAtkRange { get; }
    public bool IsAttacking { get; set; }
    public bool IsBasicAttacking { get; }
    public bool CanBasicAtk { get; }
    public bool IsSkilling { get; set; }
    public bool IsAttackLunging { get; }
    public bool IsInCombat { get; }
    public List<MobSkill> Skills { get; }
    public MobSkill ActiveSkill { get; }
    public List<MobEffect> Effects { get; }
    public abstract void OnAttackInterrupt();
    public abstract void FireProjectile();
    public abstract void AEventInstantiateFXOne();
    public abstract void AEventInstantiateFXTwo();
    public abstract void AEventEnableMainHandCollider();
    public abstract void AEventDisableMainHandCollider();
    public abstract void AEventEnableOffHandCollider();
    public abstract void AEventDisableOffHandCollider();
    public abstract void AEventEnableSkillCollider();
    public abstract void AEventDisableSkillCollider();
    public abstract void AEventChargeSkill();
    public abstract void AEventChargeCancelSkill();
    public abstract void AEventChargeReleaseSkill();
    public abstract void AEventDisableSkill();
    public abstract void AEventSummonSkill();
  }
}
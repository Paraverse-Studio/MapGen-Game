using Paraverse.Combat;
using Paraverse.Player;
using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;

public class BuffSkill : MobSkill, IMobSkill
{
  public enum BoostType
  {
    attackStatIncreaseBoost,
    abilityStatIncreaseBoost,
    healthStatIncreaseBoost,
    movementStatIncreaseBoost,
    attackDamageBoost,
    abilityPowerBoost,
    overallDamageBoost
  }

  [System.Serializable]
  public class BoostElement
  {
    public BoostType type;
    public ScalingStatData scalingValue;

    [HideInInspector] // runtime
    public StatModifier buff;

    [HideInInspector] // runtime
    public bool isPercentageBuff = false;
  }

  #region variables
  [Header("Buff Skill Properties:")]
  [SerializeField] private List<BoostElement> Buffs;
  [SerializeField] private GameObject buffVFX;
  [SerializeField] private bool vfxWeaponParent = false;
  [SerializeField] private bool vfxBodyParent = false;
  [SerializeField] private float buffDuration;
  [SerializeField] private float attackRangeLengthen = 0f;

  [Foldout("Glow/Outline"), SerializeField] private bool useGlowOutline = false;
  [Foldout("Glow/Outline"), SerializeField] private Material materialForWeapon = null;
  [Foldout("Glow/Outline"), SerializeField] private float sizeOfGlow = 0;
  [Foldout("Glow/Outline"), SerializeField] private Color colorOfGlow;
  [Foldout("Glow/Outline")] private Material _glowMaterial; // Run-time for the above

  private float _buffDurationElapsed = 0f;
  private Transform _userWeapon = null;
  private GameObject _VFX = null;
  private bool skillActive = false;
  #endregion

  public override void ActivateSkill(PlayerCombat mob, PlayerInputControls input, Animator anim, MobStats stats, Transform target = null)
  {
    base.ActivateSkill(mob, input, anim, stats, target);
    _userWeapon = attackColliderGO.transform.parent;
  }

  public override void DeactivateSkill(PlayerInputControls input)
  {
    base.DeactivateSkill(input);
    OnSkillComplete();
    Destroy(_VFX);
  }

  public override void SkillUpdate()
  {
    base.SkillUpdate();
    BuffDurationHandler();

    foreach (BoostElement buff in Buffs)
    {
      if (null != buff.buff)
      {
        buff.buff.Value = buff.scalingValue.FinalValue(stats);
        if (buff.isPercentageBuff)
        {
          buff.buff.Value /= 100.0f;
        }
      }
    }
  }

  protected override void ExecuteSkillLogic()
  {
    base.ExecuteSkillLogic();
    skillActive = true;

    _buffDurationElapsed = buffDuration;

    // Add the buff VFX and stats to the player
    if (null == _VFX && vfxWeaponParent) _VFX = Instantiate(buffVFX, _userWeapon);
    if (null == _VFX && vfxBodyParent) _VFX = Instantiate(buffVFX, stats.transform);
    ToggleGlowAndOutline(true);

    ToggleParticleSystem(turnParticlesOn: true);

    if (null == Buffs[0].buff)
    {
      ActivateBuffs();
    }

    Vector3 scale = attackColliderGO.transform.localScale;
    attackColliderGO.transform.localScale = new Vector3(scale.x, scale.y + attackRangeLengthen, scale.z);
  }

  protected override bool CanUseSkill()
  {
    if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && mob.IsAttacking == false && mob.ActiveSkill == null && mob.Controller.ActiveKnockBackEffect == null ||
      IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && mob.IsAttacking == false && skillActive == false && input != null) // For player Active skill is always active
      return true;

    return false;
  }

  private void BuffDurationHandler()
  {
    if (_buffDurationElapsed <= 0 && null != Buffs[0].buff)
    {
      OnSkillComplete();
    }
    else
    {
      _buffDurationElapsed = Mathf.Clamp(_buffDurationElapsed - Time.deltaTime, 0f, buffDuration);
    }
  }

  protected override void OnSkillComplete()
  {
    base.OnSkillComplete();
    skillActive = false;

    if (_VFX)
    {
      ToggleParticleSystem(turnParticlesOn: false);
    }

    if (null != Buffs[0].buff)
    {
      DeactivateBuffs();

      Vector3 scale = attackColliderGO.transform.localScale;
      attackColliderGO.transform.localScale = new Vector3(scale.x, scale.y - attackRangeLengthen, scale.z);
    }

    ToggleGlowAndOutline(false);
  }

  private void ToggleParticleSystem(bool turnParticlesOn)
  {
    if (null == _VFX) return;

    var list = _VFX.GetComponentsInChildren<ParticleSystem>();
    foreach (ParticleSystem ps in list)
    {
      if (turnParticlesOn) ps.Play();
      else ps.Stop();
    }
  }

  private void DeactivateBuffs()
  {
    for (int i = 0; i < Buffs.Count; ++i)
    {
      BoostElement buff = Buffs[i];
      if (null == buff.buff) continue;

      switch (buff.type)
      {
        case BoostType.attackStatIncreaseBoost:
          stats.AttackDamage.RemoveMod(buff.buff);
          break;
        case BoostType.abilityStatIncreaseBoost:
          stats.AbilityPower.RemoveMod(buff.buff);
          break;
        case BoostType.healthStatIncreaseBoost:
          float ratio = stats.CurHealth / stats.MaxHealth.FinalValue;
          stats.MaxHealth.RemoveMod(buff.buff);
          stats.SetCurrentHealth(Mathf.CeilToInt(stats.MaxHealth.FinalValue * ratio));
          break;
        case BoostType.movementStatIncreaseBoost:
          stats.MoveSpeed.RemoveMod(buff.buff);
          break;
        case BoostType.attackDamageBoost:
          stats.MobBoosts.AttackDamageBoost.RemoveMod(buff.buff);
          break;
        case BoostType.abilityPowerBoost:
          stats.MobBoosts.AbilityPowerBoost.RemoveMod(buff.buff);
          break;
        case BoostType.overallDamageBoost:
          stats.MobBoosts.OverallDamageBoost.RemoveMod(buff.buff);
          break;
      }
      buff.buff = null;
    }
  }

  private void ActivateBuffs()
  {
    for (int i = 0; i < Buffs.Count; ++i)
    {
      BoostElement buff = Buffs[i];
      buff.buff = new StatModifier(buff.scalingValue.FinalValue(stats));

      switch (buff.type)
      {
        case BoostType.attackStatIncreaseBoost:
          stats.AttackDamage.AddMod(buff.buff);
          break;
        case BoostType.abilityStatIncreaseBoost:
          stats.AbilityPower.AddMod(buff.buff);
          break;
        case BoostType.healthStatIncreaseBoost:
          stats.MaxHealth.AddMod(buff.buff);
          stats.UpdateCurrentHealth(Mathf.CeilToInt(buff.buff.Value));
          break;
        case BoostType.movementStatIncreaseBoost:
          stats.MoveSpeed.AddMod(buff.buff);
          break;
        case BoostType.attackDamageBoost:
          buff.isPercentageBuff = true;
          buff.buff.Value = (buff.buff.Value / 100.0f);
          stats.MobBoosts.AttackDamageBoost.AddMod(buff.buff);
          break;
        case BoostType.abilityPowerBoost:
          buff.isPercentageBuff = true;
          buff.buff.Value = (buff.buff.Value / 100.0f);
          stats.MobBoosts.AbilityPowerBoost.AddMod(buff.buff);
          break;
        case BoostType.overallDamageBoost:
          buff.isPercentageBuff = true;
          buff.buff.Value = (buff.buff.Value / 100.0f);
          stats.MobBoosts.OverallDamageBoost.AddMod(buff.buff);
          break;
      }
    }
  }

  private void ToggleGlowAndOutline(bool on)
  {
    if (!useGlowOutline) return;

    if (on)
    {
      var renderer = _userWeapon.gameObject.GetComponent<MeshRenderer>();
      Material[] weaponMaterials = new Material[renderer.materials.Length + 1];
      for (int i = 0; i < renderer.materials.Length; ++i)
      {
        weaponMaterials[i] = renderer.materials[i];
      }
      _glowMaterial = weaponMaterials[weaponMaterials.Length - 1] = materialForWeapon;
      renderer.materials = weaponMaterials;
    }
    else if (_glowMaterial)
    {
      var renderer = _userWeapon.gameObject.GetComponent<MeshRenderer>();
      List<Material> weaponMaterials = new();
      weaponMaterials.AddRange(renderer.materials);

      for (int i = 0; i < weaponMaterials.Count; ++i)
      {
        if (weaponMaterials[i].name.Contains(_glowMaterial.name))
        {
          weaponMaterials.Remove(weaponMaterials[i]);
        }
      }
      renderer.materials = weaponMaterials.ToArray();
    }
  }


}

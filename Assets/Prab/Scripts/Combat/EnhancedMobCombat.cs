using Paraverse;
using Paraverse.Combat;
using Paraverse.Mob.Combat;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle Mob skills here, as basic attacks are handled in the inherited MobCombat.cs. 
/// If any code in MobCombat.cs needs to be alter, just override the method within this script. 
/// </summary>
public class EnhancedMobCombat : MobCombat
{
    #region Variables
    protected int usingSkillIdx;
    [SerializeField, Tooltip("Mob skills.")]
    protected List<MobSkill> skills = new List<MobSkill>();
    [SerializeField]
    protected string animBool = "isUsingSkill";

    #region Skill One Delegates and Events
    // SKILL ONE
    // Enable/Disables main hand collider for Skill One
    public delegate void OnEnableMainHandColliderSOneDel();
    public event OnEnableMainHandColliderSOneDel OnEnableMainHandColliderSOneEvent;
    public delegate void OnDisableMainHandColliderSOneDel();
    public event OnEnableMainHandColliderSOneDel OnDisableMainHandColliderSOneEvent;
    // Enable/Disables off hand collider for Skill One
    public delegate void OnEnableOffHandColliderSOneDel();
    public event OnEnableOffHandColliderSOneDel OnEnableOffHandColliderSOneEvent;
    public delegate void OnDisableOffHandColliderSOneDel();
    public event OnDisableOffHandColliderSOneDel OnDisableOffHandColliderSOneEvent;
    // Enables/Disables special skill collider for Skill One
    public delegate void OnEnableSkillColliderSOneDel();
    public event OnEnableSkillColliderSOneDel OnEnableSkillColliderSOneEvent;
    public delegate void OnDisableSkillColliderSOneDel();
    public event OnDisableSkillColliderSOneDel OnDisableSkillColliderSOneEvent;
    // Handler special charging/releasing skills for Skill One
    public delegate void OnChargeSkillOneDel();
    public event OnChargeSkillOneDel OnChargeSkillOneEvent;
    public delegate void OnChargeCancelSkillOneDel();
    public event OnChargeCancelSkillOneDel OnChargeCancelSkillOneEvent;
    public delegate void OnEnableChargeReleaseSkillOneDel();
    public event OnEnableChargeReleaseSkillOneDel OnEnableChargeReleaseSkillOneEvent;
    // Used to disable anything at the end 
    public delegate void OnDisableSkillOneDel();
    public event OnDisableSkillOneDel OnDisableSkillOneEvent;
    // Used to summon 
    public delegate void OnSummonSkillOneDel();
    public event OnSummonSkillOneDel OnSummonSkillOneEvent;
    // Used to instantiate FXs
    public delegate void OnInstantiateFXOneDel();
    public event OnInstantiateFXOneDel OnInstantiateFXOneEvent;
    public delegate void OnSummonSkillTwoDel();
    public event OnSummonSkillTwoDel OnInstantiateFXTwoEvent;
    #endregion
    #endregion


    protected override void Start()
    {
        base.Start();
        IsSkilling = false;
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].ActivateSkill(this, anim, stats, player);
            if (skills[i].skillOn)
            {
                IsSkilling = true;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (anim.GetBool(StringData.IsUsingSkill))
            IsSkilling = true;
        else
            IsSkilling = false;

        // Gets active skill to run update method for each skill 
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].SkillUpdate();
            if (skills[i].skillOn)
            {
                usingSkillIdx = i;
            }
        }
    }

    /// <summary>
    /// Responsible for handling basic attack animation and cooldown.
    /// </summary>
    public override void BasicAttackHandler()
    {
        if (curBasicAtkCd <= 0 && IsSkilling == false && IsSkillReady() == false)
        {
            anim.Play(StringData.BasicAttack);
            curBasicAtkCd = GetBasicAttackCooldown();
        }
    }

    private bool IsSkillReady()
    {
        for (int i = 0; i < skills.Count; ++i)
        {
            if (skills[i].IsOffCooldown && skills[i].TargetWithinRange)
                return true;
        }
        return false;
    }

    #region Animation Events Skill One
    public override void FireProjectile()
    {
        ProjectileData data;
        float damage = basicAtkDmgRatio * stats.AttackDamage.FinalValue; 

        if (IsSkilling)
        {
            MobSkill s = skills[usingSkillIdx];
            data = s.projData;
            damage = s.scalingStatData.flatPower + (stats.AttackDamage.FinalValue * s.scalingStatData.attackScaling) + (stats.AbilityPower.FinalValue * s.scalingStatData.abilityScaling);
        }
        else
            data = projData;

        // Archers may hold an arrow which needs to be set to off/on when firing
        if (data.projHeld != null)
            data.projHeld.SetActive(false);

        Vector3 playerPos = new Vector3(player.position.x, player.position.y + 0.5f, player.position.z);
        Vector3 targetDir = (playerPos - transform.position).normalized;

        Quaternion lookRot;
        if (data.projRotation == null)
        {
            lookRot = Quaternion.LookRotation(targetDir);
        }
        else
        {
            lookRot = data.projRotation.rotation;
        }

        // Instantiate and initialize projectile
        GameObject go = Instantiate(data.projPf, data.projOrigin.position, lookRot);
        Projectile proj = go.GetComponent<Projectile>();
        proj.Init(this, targetDir, projData.basicAtkProjSpeed, basicAtkRange, damage);
    }

    public virtual void AEventInstantiateFXOne()
    {
        OnInstantiateFXOneEvent?.Invoke();
    }

    public virtual void AEventInstantiateFXTwo()
    {
        OnInstantiateFXTwoEvent?.Invoke();
    }

    public virtual void AEventEnableMainHandCollider()
    {
        OnEnableMainHandColliderSOneEvent?.Invoke();
    }

    public virtual void AEventDisableMainHandCollider()
    {
        OnDisableMainHandColliderSOneEvent?.Invoke();
    }

    public virtual void AEventEnableOffHandCollider()
    {
        OnEnableOffHandColliderSOneEvent?.Invoke();
    }

    public virtual void AEventDisableOffHandCollider()
    {
        OnDisableOffHandColliderSOneEvent?.Invoke();
    }

    public virtual void AEventEnableSkillCollider()
    {
        OnEnableSkillColliderSOneEvent?.Invoke();
    }

    public virtual void AEventDisableSkillCollider()
    {
        OnDisableSkillColliderSOneEvent?.Invoke();
    }

    public virtual void AEventChargeSkill()
    {
        OnChargeSkillOneEvent?.Invoke();
    }

    public virtual void AEventChargeCancelSkill()
    {
        OnChargeCancelSkillOneEvent?.Invoke();
    }

    public virtual void AEventChargeReleaseSkill()
    {
        OnEnableChargeReleaseSkillOneEvent?.Invoke();
    }

    public virtual void AEventDisableSkill()
    {
        OnDisableSkillOneEvent?.Invoke();
    }

    public virtual void AEventSummonSkill()
    {
        OnSummonSkillOneEvent?.Invoke();
    }
    #endregion
}

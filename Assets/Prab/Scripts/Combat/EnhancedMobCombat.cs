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

    public delegate void OnEnableMainHandColliderDel();
    public event OnEnableMainHandColliderDel OnEnableMainHandColliderEvent;
    public delegate void OnDisableMainHandColliderDel();
    public event OnEnableMainHandColliderDel OnDisableMainHandColliderEvent;
    public delegate void OnEnableOffHandColliderDel();
    public event OnEnableOffHandColliderDel OnEnableOffHandColliderEvent;
    public delegate void OnDisableOffHandColliderDel();
    public event OnDisableOffHandColliderDel OnDisableOffHandColliderEvent;
    public delegate void OnEnableSkillColliderDel();
    public event OnEnableSkillColliderDel OnEnableSkillColliderEvent;
    public delegate void OnDisableSkillColliderDel();
    public event OnDisableSkillColliderDel OnDisableSkillColliderEvent;
    public delegate void OnChargeSkillDel();
    public event OnChargeSkillDel OnChargeSkillEvent;
    public delegate void OnChargeCancelSkillDel();
    public event OnChargeSkillDel OnChargeCancelSkillEvent;
    public delegate void OnEnableChargeReleaseSkillDel();
    public event OnEnableChargeReleaseSkillDel OnEnableChargeReleaseSkillEvent;
    public delegate void OnDisableSkillDel();
    public event OnDisableSkillDel OnDisableSkillEvent;
    public delegate void OnSummonSkillDel();
    public event OnSummonSkillDel OnSummonSkillEvent;
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
        if (curBasicAtkCd <= 0 && IsSkilling == false)
        {
            anim.Play(StringData.BasicAttack);
            curBasicAtkCd = GetBasicAttackCooldown();
        }
    }

    #region Animation Events
    public override void FireProjectile()
    {
        ProjectileData data;
        if (anim.GetBool(StringData.IsUsingSkill))
            data = skills[usingSkillIdx].projData;
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
        proj.Init(this, targetDir, projData.basicAtkProjSpeed, basicAtkRange, basicAtkDmgRatio * stats.AttackDamage.FinalValue);
    }

    public virtual void AEventEnableMainHandCollider()
    {
        OnEnableMainHandColliderEvent?.Invoke();
    }

    public virtual void AEventDisableMainHandCollider()
    {
        OnDisableMainHandColliderEvent?.Invoke();
    }

    public virtual void AEventEnableOffHandCollider()
    {
        OnEnableOffHandColliderEvent?.Invoke();
    }

    public virtual void AEventDisableOffHandCollider()
    {
        OnDisableOffHandColliderEvent?.Invoke();
    }

    public virtual void AEventEnableSkillCollider()
    {
        OnEnableSkillColliderEvent?.Invoke();
    }

    public virtual void AEventDisableSkillCollider()
    {
        OnDisableSkillColliderEvent?.Invoke();
    }

    public virtual void AEventChargeSkill()
    {
        OnChargeSkillEvent?.Invoke();
    }

    public virtual void AEventChargeCancelSkill()
    {
        OnChargeCancelSkillEvent?.Invoke();
    }

    public virtual void AEventChargeReleaseSkill()
    {
        OnEnableChargeReleaseSkillEvent?.Invoke();
    }

    public virtual void AEventDisableSkill()
    {
        OnDisableSkillEvent?.Invoke();
    }

    public virtual void AEventSummonSkill()
    {
        OnSummonSkillEvent?.Invoke();
    }
    #endregion
}

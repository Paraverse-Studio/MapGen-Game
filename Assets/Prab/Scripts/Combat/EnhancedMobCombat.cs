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

    //#region Skill Two Delegates and Events
    //// SKILL Two
    //// Enable/Disables main hand collider for Skill One
    //public delegate void OnEnableMainHandColliderSTwoDel();
    //public event OnEnableMainHandColliderSTwoDel OnEnableMainHandColliderSTwoEvent;
    //public delegate void OnDisableMainHandColliderSTwoDel();
    //public event OnDisableMainHandColliderSTwoDel OnDisableMainHandColliderSTwoEvent;
    //// Enable/Disables off hand collider for Skill One
    //public delegate void OnEnableOffHandColliderSTwoDel();
    //public event OnEnableOffHandColliderSTwoDel OnEnableOffHandColliderSTwoEvent;
    //public delegate void OnDisableOffHandColliderSTwoDel();
    //public event OnDisableOffHandColliderSTwoDel OnDisableOffHandColliderSTwoEvent;
    //// Enables/Disables special skill collider for Skill One
    //public delegate void OnEnableSkillColliderSTwoDel();
    //public event OnEnableSkillColliderSTwoDel OnEnableSkillColliderSTwoEvent;
    //public delegate void OnDisableSkillColliderSTwoDel();
    //public event OnDisableSkillColliderSTwoDel OnDisableSkillColliderSTwoEvent;
    //// Handler special charging/releasing skills for Skill One
    //public delegate void OnChargeSkillTwoDel();
    //public event OnChargeSkillTwoDel OnChargeSkillTwoEvent;
    //public delegate void OnChargeCancelSkillTwoDel();
    //public event OnChargeCancelSkillTwoDel OnChargeCancelSkillTwoEvent;
    //public delegate void OnEnableChargeReleaseSkillTwoDel();
    //public event OnEnableChargeReleaseSkillTwoDel OnEnableChargeReleaseSkillTwoEvent;
    //// Used to disable anything at the end 
    //public delegate void OnDisableSkillTwoDel();
    //public event OnDisableSkillTwoDel OnDisableSkillTwoEvent;
    //// Used to summon 
    //public delegate void OnSummonSkillTwoDel();
    //public event OnSummonSkillTwoDel OnSummonSkillTwoEvent;
    //#endregion

    //#region Skill Three Delegates and Events
    //// SKILL THREE
    //// Enable/Disables main hand collider for Skill One
    //public delegate void OnEnableMainHandColliderSThreeDel();
    //public event OnEnableMainHandColliderSThreeDel OnEnableMainHandColliderSThreeEvent;
    //public delegate void OnDisableMainHandColliderSThreeDel();
    //public event OnDisableMainHandColliderSThreeDel OnDisableMainHandColliderSThreeEvent;
    //// Enable/Disables off hand collider for Skill One
    //public delegate void OnEnableOffHandColliderSThreeDel();
    //public event OnEnableOffHandColliderSThreeDel OnEnableOffHandColliderSThreeEvent;
    //public delegate void OnDisableOffHandColliderSThreeDel();
    //public event OnDisableOffHandColliderSThreeDel OnDisableOffHandColliderSThreeEvent;
    //// Enables/Disables special skill collider for Skill One
    //public delegate void OnEnableSkillColliderSThreeDel();
    //public event OnEnableSkillColliderSThreeDel OnEnableSkillColliderSThreeEvent;
    //public delegate void OnDisableSkillColliderSThreeDel();
    //public event OnDisableSkillColliderSThreeDel OnDisableSkillColliderSThreeEvent;
    //// Handler special charging/releasing skills for Skill One
    //public delegate void OnChargeSkillThreeDel();
    //public event OnChargeSkillOneDel OnChargeSkillThreeEvent;
    //public delegate void OnChargeCancelSkillThreeDel();
    //public event OnChargeCancelSkillThreeDel OnChargeCancelSkillThreeEvent;
    //public delegate void OnEnableChargeReleaseSkillThreeDel();
    //public event OnEnableChargeReleaseSkillThreeDel OnEnableChargeReleaseSkillThreeEvent;
    //// Used to disable anything at the end 
    //public delegate void OnDisableSkillThreeDel();
    //public event OnDisableSkillThreeDel OnDisableSkillThreeEvent;
    //// Used to summon 
    //public delegate void OnSummonSkillThreeDel();
    //public event OnSummonSkillThreeDel OnSummonSkillThreeEvent;
    //#endregion
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

    #region Animation Events Skill One
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

    //#region Animation Events Skill Two
    //public virtual void AEventEnableMainHandColliderSkillTwo()
    //{
    //    OnEnableMainHandColliderSTwoEvent?.Invoke();
    //}

    //public virtual void AEventDisableMainHandColliderSkillTwo()
    //{
    //    OnDisableMainHandColliderSTwoEvent?.Invoke();
    //}

    //public virtual void AEventEnableOffHandColliderSkillTwo()
    //{
    //    OnEnableOffHandColliderSTwoEvent?.Invoke();
    //}

    //public virtual void AEventDisableOffHandColliderSkillTwo()
    //{
    //    OnDisableOffHandColliderSTwoEvent?.Invoke();
    //}

    //public virtual void AEventEnableSkillTwoCollider()
    //{
    //    OnEnableSkillColliderSTwoEvent?.Invoke();
    //}

    //public virtual void AEventDisableSkillTwoCollider()
    //{
    //    OnDisableSkillColliderSTwoEvent?.Invoke();
    //}

    //public virtual void AEventChargeSkillTwo()
    //{
    //    OnChargeSkillTwoEvent?.Invoke();
    //}

    //public virtual void AEventChargeCancelSkillTwo()
    //{
    //    OnChargeCancelSkillTwoEvent?.Invoke();
    //}

    //public virtual void AEventChargeReleaseSkillTwo()
    //{
    //    OnEnableChargeReleaseSkillTwoEvent?.Invoke();
    //}

    //public virtual void AEventDisableSkillTwo()
    //{
    //    OnDisableSkillTwoEvent?.Invoke();
    //}

    //public virtual void AEventSummonSkillTwo()
    //{
    //    OnSummonSkillTwoEvent?.Invoke();
    //}
    //#endregion

    //#region Animation Events Skill Three
    //public virtual void AEventEnableMainHandColliderSkillThree()
    //{
    //    OnEnableMainHandColliderSThreeEvent?.Invoke();
    //}

    //public virtual void AEventDisableMainHandColliderSkillThree()
    //{
    //    OnDisableMainHandColliderSThreeEvent?.Invoke();
    //}

    //public virtual void AEventEnableOffHandColliderSkillThree()
    //{
    //    OnEnableOffHandColliderSThreeEvent?.Invoke();
    //}

    //public virtual void AEventDisableOffHandColliderSkillThree()
    //{
    //    OnDisableOffHandColliderSThreeEvent?.Invoke();
    //}

    //public virtual void AEventEnableSkillThreeCollider()
    //{
    //    OnEnableSkillColliderSThreeEvent?.Invoke();
    //}

    //public virtual void AEventDisableSkillThreeCollider()
    //{
    //    OnDisableSkillColliderSThreeEvent?.Invoke();
    //}

    //public virtual void AEventChargeSkillThree()
    //{
    //    OnChargeSkillThreeEvent?.Invoke();
    //}

    //public virtual void AEventChargeCancelSkillThree()
    //{
    //    OnChargeCancelSkillThreeEvent?.Invoke();
    //}

    //public virtual void AEventChargeReleaseSkillThree()
    //{
    //    OnEnableChargeReleaseSkillThreeEvent?.Invoke();
    //}

    //public virtual void AEventDisableSkillThree()
    //{
    //    OnDisableSkillThreeEvent?.Invoke();
    //}

    //public virtual void AEventSummonSkillThree()
    //{
    //    OnSummonSkillThreeEvent?.Invoke();
    //}
    //#endregion
}

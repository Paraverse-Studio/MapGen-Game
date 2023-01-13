using Paraverse.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class SlamDownPunchSkill : MobSkill, IMobSkill
{
    #region Variables
    protected MobController controller;
    #endregion

    #region Inherited Methods
    public override void ActivateSkill(EnhancedMobCombat mob, Animator anim, IMobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, anim, stats, target);

        controller = mob.GetComponent<MobController>();

        mob.OnChargeSkillEvent += ChargePunch;
        mob.OnChargeCancelSkillEvent += ChargePunchCancel;
        mob.OnEnableChargeReleaseSkillEvent += UnleashPunch;
        mob.OnEnableOffHandColliderEvent += EnableOffHandAttackCollider;
        mob.OnDisableOffHandColliderEvent += DisableOffHandAttackCollider;
    }

    public override void DeactivateSkill(PlayerInputControls input)
    {
        base.DeactivateSkill(input);

        mob.OnEnableMainHandColliderEvent -= ChargePunch;
        mob.OnDisableMainHandColliderEvent -= ChargePunchCancel;
        mob.OnEnableOffHandColliderEvent -= UnleashPunch;
        mob.OnDisableOffHandColliderEvent -= EnableOffHandAttackCollider;
        mob.OnDisableSkillEvent -= DisableOffHandAttackCollider;
    }
    #endregion

    #region Animation Events
    public void ChargePunch()
    {
        controller.ApplyFlyUp(mob.transform.position);
    }

    public void ChargePunchCancel()
    {
        controller.isFlyingUp = false;
    }

    public void UnleashPunch()
    {
        controller.ApplyFlyDown(target.transform.position);
    }

    public void EnableOffHandAttackCollider()
    {
        if (attackColliderGO != null)
            attackColliderGO.SetActive(true);
    }

    public void DisableOffHandAttackCollider()
    {
        if (attackColliderGO != null)
            attackColliderGO.SetActive(false);
    }
    #endregion
}

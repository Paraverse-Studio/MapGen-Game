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

        if (controller == null)
            controller = mob.GetComponent<MobController>();
    }

    public override void DeactivateSkill(PlayerInputControls input)
    {
        base.DeactivateSkill(input);
    }

    public override void SubscribeAnimationEventListeners()
    {
        base.SubscribeAnimationEventListeners();

        mob.OnChargeSkillOneEvent += ChargePunch;
        mob.OnChargeCancelSkillOneEvent += ChargePunchCancel;
        mob.OnEnableChargeReleaseSkillOneEvent += UnleashPunch;
        mob.OnEnableOffHandColliderSOneEvent += EnableOffHandAttackCollider;
        mob.OnDisableOffHandColliderSOneEvent += DisableOffHandAttackCollider;
    }

    public override void UnsubscribeAnimationEventListeners()
    {
        base.UnsubscribeAnimationEventListeners();

        mob.OnEnableMainHandColliderSOneEvent -= ChargePunch;
        mob.OnDisableMainHandColliderSOneEvent -= ChargePunchCancel;
        mob.OnEnableOffHandColliderSOneEvent -= UnleashPunch;
        mob.OnDisableOffHandColliderSOneEvent -= EnableOffHandAttackCollider;
        mob.OnDisableSkillOneEvent -= DisableOffHandAttackCollider;
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

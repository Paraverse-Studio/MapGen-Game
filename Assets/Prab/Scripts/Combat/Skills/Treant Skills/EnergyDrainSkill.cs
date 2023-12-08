using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrainSkill : MobSkill, IMobSkill
{
    #region Variables
    [SerializeField, Range(0,1)]
    protected float lifeStealRatio = 0.3f;
    #endregion

    #region Inherited Methods
    public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, anim, stats, target);
        attackCollider.Init(mob, stats, scalingStatData);
        attackCollider.OnBasicAttackApplyDamageEvent += ApplyLiftSteal;
        mob.OnEnableSkillColliderSOneEvent += EnableCollider;
        mob.OnDisableSkillColliderSOneEvent += DisableCollider;
    }

    public override void DeactivateSkill(PlayerInputControls input)
    {
        base.DeactivateSkill(input);
        attackCollider.OnBasicAttackApplyDamageEvent -= ApplyLiftSteal;
        mob.OnEnableSkillColliderSOneEvent -= EnableCollider;
        mob.OnDisableSkillColliderSOneEvent -= DisableCollider;

    }
    #endregion

    #region Private Methods
    private void ApplyLiftSteal(float dmg)
    {
        float healAmount = dmg * lifeStealRatio;
        stats.UpdateCurrentHealth((int)healAmount);
        Debug.Log(transform.name + "applied " + dmg + " points of damage and healed for " + healAmount + " points of health.  LS Ratio: " + lifeStealRatio);
    }
    #endregion

    #region Animation Events
    public void EnableCollider()
    {
        attackColliderGO.SetActive(true);
    }

    public void DisableCollider()
    {
        attackColliderGO.SetActive(false);
        DisableSkill();
    }
    #endregion
}

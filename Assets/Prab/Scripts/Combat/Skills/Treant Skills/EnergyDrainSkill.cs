using Paraverse.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrainSkill : MobSkill, IMobSkill
{
    #region Variables
    [SerializeField]
    protected float lifeStealRatio = 0.3f;
    #endregion

    #region Inherited Methods
    public override void ActivateSkill(EnhancedMobCombat mob, Animator anim, IMobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, anim, stats, target);
        attackCollider.Init(mob, stats, null, true);
        mob.basicAttackCollider.OnBasicAttackApplyDamageEvent += ApplyLiftSteal;
        mob.OnDisableSkillOneEvent += DisableSkill;
    }

    public override void DeactivateSkill(PlayerInputControls input)
    {
        base.DeactivateSkill(input);
        mob.basicAttackCollider.OnBasicAttackApplyDamageEvent -= ApplyLiftSteal;
        mob.OnDisableSkillOneEvent -= DisableSkill;
    }
    #endregion

    #region Private Methods
    private void ApplyLiftSteal(float dmg)
    {
        float healAmount = dmg * lifeStealRatio;
        stats.UpdateCurrentHealth((int)healAmount);
    }
    #endregion
}

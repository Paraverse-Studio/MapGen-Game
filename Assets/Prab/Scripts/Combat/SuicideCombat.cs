using Paraverse;
using Paraverse.Helper;
using Paraverse.Mob.Combat;
using UnityEngine;

public class SuicideCombat : MobCombat
{
    [SerializeField]
    private GameObject explosionEffect;
    [SerializeField]
    private float explosionRadius = 3f;
    [SerializeField]
    private ScalingStatData scalingStatData;


    protected override void Update()
    {
        if (controller.IsDead) return;

        distanceFromTarget = ParaverseHelper.GetDistance(ParaverseHelper.GetPositionXZ(transform.position), ParaverseHelper.GetPositionXZ(player.position));
        _isBasicAttacking = anim.GetBool(StringData.IsBasicAttacking);
        //AttackCooldownHandler();
        if (IsBasicAttacking && distanceFromTarget <= explosionRadius)
        {
            Explode();
        }
        basicAttackSkill.SkillUpdate();
    }

    private void Explode()
    {
        GameObject go = Instantiate(explosionEffect, transform.position, transform.rotation);
        AttackCollider col = go.GetComponentInChildren<AttackCollider>();
        col.Init(this, stats, scalingStatData);
        stats.UpdateCurrentHealth(-10000000);
    }
}

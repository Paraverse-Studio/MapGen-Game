using Paraverse;
using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;

public class SuicideCombat : MobCombat
{
    [SerializeField]
    private GameObject explosionEffect;
    [SerializeField]
    private float explosionRadius = 3f;


    protected override void Update()
    {
        if (controller.IsDead) return;

        distanceFromTarget = ParaverseHelper.GetDistance(ParaverseHelper.GetPositionXZ(transform.position), ParaverseHelper.GetPositionXZ(player.position));
        _isBasicAttacking = anim.GetBool(StringData.IsBasicAttacking);
        AttackCooldownHandler();
        if (IsBasicAttacking && distanceFromTarget <= explosionRadius)
        {
            Explode();
        }
    }

    public override void BasicAttackHandler()
    {
        if (curBasicAtkCd <= 0)
        {
            anim.Play(StringData.BasicAttack);
            curBasicAtkCd = GetBasicAttackCooldown();
        }
    }

    private void Explode()
    {
        GameObject go = Instantiate(explosionEffect, transform.position, transform.rotation);
        AttackCollider col = go.GetComponentInChildren<AttackCollider>();
        col.Init(this, stats);
        stats.UpdateCurrentHealth(-10000000);
    }
}

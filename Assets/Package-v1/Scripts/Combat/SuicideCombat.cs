using Paraverse.Mob.Combat;
using UnityEngine;

public class SuicideCombat : MobCombat
{
    [SerializeField]
    private GameObject explosionEffect;

    public override void BasicAttackHandler()
    {
        if (curBasicAtkCd <= 0)
        {
            anim.Play(StringData.BasicAttack);
            curBasicAtkCd = GetBasicAttackCooldown();
        }
        if (IsBasicAttacking)
        {
            if (distanceFromTarget <= 2f)
                Explode();
        }
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

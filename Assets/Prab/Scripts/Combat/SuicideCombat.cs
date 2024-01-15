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
    if (IsBasicAttacking && distanceFromTarget <= explosionRadius)
    {
      Explode();
    }
    basicAttackSkill.SkillUpdate();
  }

  //private void OnTriggerEnter(Collider other)
  //{
  //  if (false == IsBasicAttacking) return;
  //  Debug.Log($"other: {other}");
  //  if (other.CompareTag(StringData.PlayerTag))
  //  {
  //    Explode();
  //  }
  //}

  private void Explode()
  {
    GameObject go = Instantiate(explosionEffect, transform.position, transform.rotation);
    AttackCollider col = go.GetComponentInChildren<AttackCollider>();
    col.Init(this, scalingStatData);
    col.gameObject.SetActive(true);
    stats.UpdateCurrentHealth(-stats.CurHealth);
  }
}

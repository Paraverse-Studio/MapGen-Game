using Paraverse;
using Paraverse.Combat;
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
    base.Update();

    if (IsBasicAttacking && distanceFromTarget <= explosionRadius)
    {
      Explode();
    }

    if (IsAttacking == false && Skills[0].SkillState.Equals(SkillState.InUse))
      Skills[0].SetSkillState(SkillState.Used);
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

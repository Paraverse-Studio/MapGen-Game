using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class DashAttackEffect : MobEffect
{
  private PlayerController controller;
  private SphereCollider _col;

  [SerializeField]
  private float[] effectRadius;


  public override void ActivateEffect(MobStats stats, int id, int level)
  {
    base.ActivateEffect(stats, id, level);
    controller = _stats.GetComponent<PlayerController>();
    
    _effectNameDB = ParaverseWebsite.Models.EffectName.SweepingDash;

    if (null == _col) _col = gameObject.AddComponent<SphereCollider>();
    _col.gameObject.SetActive(true);
    _col.radius = effectRadius[effectLevel-1];
    _col.isTrigger = true;
    gameObject.transform.SetParent(_stats.transform);
    gameObject.transform.localPosition = Vector3.zero;
    _col.enabled = false;

    controller.OnStartDiveEvent += EnableCollider;
    controller.OnEndDiveEvent += DisableCollider;
  }

  public override void DeactivateEffect()
  {
    base.DeactivateEffect();

    // Remove all instantiated colliders
    if (_col) Destroy(_col.gameObject);

    controller.OnStartDiveEvent -= EnableCollider;
    controller.OnEndDiveEvent -= DisableCollider;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag(StringData.EnemyTag))
    {
      DamageLogic(other);
    }
  }

  private void EnableCollider()
  {
    _col.enabled = true;
  }

  private void DisableCollider()
  {
    _col.enabled = false;
  }
}

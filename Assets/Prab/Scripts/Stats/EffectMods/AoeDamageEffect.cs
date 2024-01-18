using Paraverse.Mob.Stats;
using UnityEngine;


public class AoeDamageEffect : MobEffect
{
  protected float timer = 0f;
  protected bool applyHit = false;

  [Header("Effect Properties")]
  protected CapsuleCollider _col;
  [SerializeField]
  protected float[] attackPerUnitOfTime;
  [SerializeField]
  protected float[] _effectRadius;

  public override void ActivateEffect(MobStats stats)
  {
    base.ActivateEffect(stats);
    
    _effectNameDB = ParaverseWebsite.Models.EffectName.Sunfire;
    
    if (null == _col) _col = gameObject.AddComponent<CapsuleCollider>();
    _col.gameObject.SetActive(true);
    _col.radius = _effectRadius[effectLevel-1];
    _col.isTrigger = true;
    gameObject.transform.SetParent(_stats.transform);
    gameObject.transform.localPosition = Vector3.zero;

    if (effectFX) _FX = Instantiate(effectFX, stats.transform);
  }

  public override void DeactivateEffect()
  {
    base.DeactivateEffect();

    // Remove all instantiated colliders
    if (_col) Destroy(_col.gameObject);
  }

  private void Update()
  {
    if (isActive == false) return;

    _col.radius = _effectRadius[effectLevel - 1];

    if (timer <= 0)
    {
      applyHit = true;
      hitTargets.Clear();
      timer = attackPerUnitOfTime[effectLevel-1];
    }
    else
      timer -= Time.deltaTime;
  }

  private void OnTriggerStay(Collider other)
  {
    if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject) && applyHit)
    {
      DamageLogic(other);
      timer = attackPerUnitOfTime[effectLevel - 1];
      hitTargets.Add(other.gameObject);
      applyHit = false;

      Debug.Log(other.name + " took " + _stats.AttackDamage.FinalValue + " points of damage.");
    }
  }
}

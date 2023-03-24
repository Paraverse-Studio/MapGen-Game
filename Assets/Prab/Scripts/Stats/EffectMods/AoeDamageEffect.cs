using Paraverse.Mob.Stats;
using System.Collections.Generic;
using UnityEngine;


public class AoeDamageEffect : MobEffect
{
    [Header("Damage Over Time Properties")]
    [SerializeField]
    protected float attackPerUnitOfTime = 1f;
    protected float timer = 0f;
    protected bool applyHit = false;
    
    [Header("Effect Properties")]
    [SerializeField]
    protected CapsuleCollider _col = null;
    [SerializeField]
    protected float _effectRadius = 3f;

    public override void ActivateEffect(MobStats stats)
    {
        base.ActivateEffect(stats);
        if (null == _col)
        {
            _col = gameObject.AddComponent<CapsuleCollider>();
        }
        _col.gameObject.SetActive(true);
        _col.radius = _effectRadius;
        _col.isTrigger = true;

        if (effectFX) _FX = Instantiate(effectFX, stats.transform);
    }

    public override void DeactivateEffect()
    {
        base.DeactivateEffect();
        _col.gameObject.SetActive(false);
        if (_FX) Destroy(_FX.gameObject);
    }

    private void Update()
    {
        if (isActive == false) return;

        _col.radius = _effectRadius;

        if (timer <= 0)
        {
            applyHit = true;
            hitTargets.Clear();
            timer = attackPerUnitOfTime;
        }
        else
            timer -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject) && applyHit)
        {
            DamageLogic(other);
            timer = attackPerUnitOfTime;
            hitTargets.Add(other.gameObject);
            applyHit = false;

            Debug.Log(other.name + " took " + _stats.AttackDamage.FinalValue + " points of damage.");
        }
    }
}

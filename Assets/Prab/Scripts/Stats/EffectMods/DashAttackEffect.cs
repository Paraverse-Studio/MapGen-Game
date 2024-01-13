using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class DashAttackEffect : MobEffect
{
    private PlayerController controller;
    private SphereCollider _col;
    [SerializeField]
    private float hitRadius = 2f;


    public override void ActivateEffect(MobStats stats)
    {
        _effectNameDB = ParaverseWebsite.Models.EffectName.SweepingDash;
        _stats = stats;
        controller = _stats.GetComponent<PlayerController>();
        _combat = _stats.GetComponent<PlayerCombat>();
        isActive = true;

        if (null == _col) _col = gameObject.AddComponent<SphereCollider>();
        _col.center += Vector3.up;
        _col.radius = hitRadius;
        _col.isTrigger = true;
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

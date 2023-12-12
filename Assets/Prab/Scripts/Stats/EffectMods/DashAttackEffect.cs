using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class DashAttackEffect : MobEffect
{
    private PlayerController controller;
    private SphereCollider col;
    [SerializeField]
    private float hitRadius = 2f;


    public override void ActivateEffect(MobStats stats)
    {
        _stats = stats;
        controller = _stats.GetComponent<PlayerController>();
        _combat = _stats.GetComponent<PlayerCombat>();
        isActive = true;

        if (null == col) col = gameObject.AddComponent<SphereCollider>();
        col.radius = hitRadius;
        col.isTrigger = true;
        col.enabled = false;

        controller.OnStartDiveEvent += EnableCollider;
        controller.OnEndDiveEvent += DisableCollider;
    }

    public override void DeactivateEffect()
    {
        base.DeactivateEffect();

        if (null != col) Destroy(col.gameObject);

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
        col.enabled = true;
    }

    private void DisableCollider()
    {
        col.enabled = false;
    }
}

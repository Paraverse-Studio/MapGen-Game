using Paraverse.Mob;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using System.Collections.Generic;
using UnityEngine;


public abstract class MobEffect : MonoBehaviour
{
    protected MobStats _stats;
    protected PlayerCombat _combat;

    [SerializeField]
    protected string targetTag = StringData.EnemyTag;

    [Header("Damage Over Time Properties")]
    [SerializeField]
    protected float attackPerUnitOfTime = 1f;
    protected float timer = 0f;
    protected bool applyHit = false;
    protected List<GameObject> hitTargets = new List<GameObject>();

    [SerializeField]
    protected ScalingStatData _empoweredScaling;

    [SerializeField]
    protected GameObject hitFX;

    public bool IsActive { get { return isActive; } }
    protected bool isActive = false;


    public virtual void ActivateEffect(MobStats stats)
    {
        _stats = stats;
        _combat = _stats.GetComponent<PlayerCombat>();
        isActive = true;
    }

    public virtual void DeactivateEffect()
    {
        isActive = false;
    }


    /// <summary>
    /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
    /// </summary>
    public virtual float ApplyCustomDamage(IMobController controller)
    {
        float totalDmg =
            _empoweredScaling.FinalValue(_stats);

        controller.Stats.UpdateCurrentHealth(-Mathf.CeilToInt(totalDmg));
        return totalDmg;
    }

    protected virtual void DamageLogic(Collider other)
    {
        hitTargets.Add(other.gameObject);

        // Enemy-related logic
        if (other.TryGetComponent(out IMobController controller))
        {
            // Apply damage
            ApplyCustomDamage(controller);
        }

        // General VFX logic
        if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

        Debug.Log(other.name + " took " + _stats.AttackDamage.FinalValue + " points of damage.");
    }
}

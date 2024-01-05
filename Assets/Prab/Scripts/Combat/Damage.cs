using Paraverse.Mob;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour, IDamage
{
    #region Variables
    protected MobCombat mob;

    [SerializeField, Tooltip("Enter the tag of target.")]
    protected string targetTag = "Player";

    [Header("Knockback Effect")]
    [SerializeField]
    protected KnockBackEffect knockBackEffect;

    [SerializeField, Tooltip("Damage over time.")]
    protected bool dot = false;
    [SerializeField, Tooltip("Apply damage upon enter.")]
    protected bool dontApplyDamageOnEnter = false;
    [SerializeField, Tooltip("Applies damage every given second.")]
    protected float dotIntervalTimer = 1f;
    protected float dotTimer = 0f;
    protected bool applyHit = false;

    protected List<GameObject> hitTargets = new List<GameObject>();
    
    [SerializeField, Tooltip("Applies hit animation to target.")]
    protected bool applyHitAnim = true;

    [Header("VFX")]
    public GameObject hitFX;

    [SerializeField]
    protected ScalingStatData scalingStatData;

    [SerializeField]
    protected bool isBasicAttackCollider = false;
    public bool IsBasicAttackCollider { get { return IsBasicAttackCollider; } }

    // Basic Attack Events Pre/During/Post
    public delegate void OnBasicLandPreDmgDel();
    public event OnBasicLandPreDmgDel OnBasicAttackPreHitEvent;
    public delegate void OnBasicAttackApplyDamageDel(float dmg);
    public event OnBasicAttackApplyDamageDel OnBasicAttackApplyDamageEvent;
    public delegate void OnBasicAttackLandPostDmgDel();
    public event OnBasicAttackLandPostDmgDel OnBasicAttackPostHitEvent;

    // Attack Events Pre/During/Post
    public delegate void OnLandPreDmgDel();
    public event OnLandPreDmgDel OnAttackPreHitEvent;
    public delegate void OnAttackApplyDamageDel(float dmg);
    public event OnAttackApplyDamageDel OnAttackApplyDamageEvent;
    public delegate void OnAttackLandPostDmgDel();
    public event OnAttackLandPostDmgDel OnAttackPostHitEvent;
    #endregion

    #region Start & Update
    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (dotTimer <= 0)
        {
            applyHit = true;
            hitTargets.Clear();
            dotTimer = dotIntervalTimer;
        }
        else
            dotTimer -= Time.deltaTime;
    }
    #endregion

    protected virtual void OnEnable()
    {
        hitTargets.Clear();
    }

    public virtual void Init(MobCombat mob)
    {
        this.mob = mob;
    }

    public virtual void Init(MobCombat mob, ScalingStatData statData)
    {
        this.mob = mob;
        scalingStatData = statData;
    }

    /// <summary>
    /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
    /// </summary>
    protected virtual float ApplyCustomDamage(IMobController controller)
    {
        float totalDmg = Mathf.CeilToInt(scalingStatData.FinalValueWithBoosts(mob.stats));
        controller.Stats.UpdateCurrentHealth(-(int)totalDmg);
        Debug.Log("Applied " + totalDmg + " points of damage to " + controller.Transform.name);
        return totalDmg;
    }

    protected virtual float ApplyCustomDamage(IMobStats stats)
    {
        float totalDmg = Mathf.CeilToInt(scalingStatData.FinalValueWithBoosts(mob.stats));
        stats.UpdateCurrentHealth(-(int)totalDmg);
        return totalDmg;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (dontApplyDamageOnEnter == true) return;

        // Detecting type of object/enemy hit
        if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject))
        {
            DamageLogic(other);
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (dot == false) return;

        if (other.CompareTag(targetTag) && !hitTargets.Contains(other.gameObject) && applyHit && dotTimer >= dotIntervalTimer && dot)
        {
            DamageLogic(other);
            dotTimer = dotIntervalTimer;
            hitTargets.Add(other.gameObject);
            applyHit = false;

            Debug.Log(other.name + " took " + mob.stats.AttackDamage.FinalValue + " points of DOT damage.");
        }
    }

    protected virtual void DamageLogic(Collider other)
    {
        InvokePreHitEvent();
        hitTargets.Add(other.gameObject);

        // Enemy-related logic
        if (other.TryGetComponent(out IMobController controller))
        {
            // Apply damage
            float dmg = ApplyCustomDamage(controller);

            InvokeApplyDamageEvent(dmg);

            // Apply knock back effect
            if (null != knockBackEffect)
            {
                KnockBackEffect effect = new KnockBackEffect(knockBackEffect);
                controller.ApplyKnockBack(mob.transform.position, effect);
            }
            else if (applyHitAnim)
            {
                controller.ApplyHitAnimation();
            }
        }

        // some entities that the player can inflict damage to may not be mobs specifically
        else if (other.TryGetComponent(out IMobStats stats))
        { 
            float dmg = ApplyCustomDamage(stats);
            InvokeApplyDamageEvent(dmg);
        }

        // General VFX logic
        if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

        InvokePostHitEvent();

        Debug.Log(other.name + " took " + scalingStatData.FinalValue(mob.stats) + " points of damage.");
    }

    protected void InvokePreHitEvent()
    {
        // Pre Basic Attack Hit Event
        if (isBasicAttackCollider)
        {
            OnBasicAttackPreHitEvent?.Invoke();
            //Debug.Log("Invoked OnBasicAttackPreEvent");
        }
        else
        {
            OnAttackPreHitEvent?.Invoke();
            //Debug.Log("Invoked OnAttackPreEvent");
        }
    }

    protected void InvokeApplyDamageEvent(float dmg)
    {
        // On Damage Applied Event
        if (isBasicAttackCollider)
        {
            OnBasicAttackApplyDamageEvent?.Invoke(dmg);
            //Debug.Log("Invoked OnBasicAttackApplyDamageEvent");
        }
        else
        {
            OnAttackApplyDamageEvent?.Invoke(dmg);
            //Debug.Log("Invoked OnAttackApplyDamageEvent");
        }
    }

    protected void InvokePostHitEvent()
    {
        // Post Basic Attack Hit Event
        if (isBasicAttackCollider)
        {
            OnBasicAttackPostHitEvent?.Invoke();
            //Debug.Log("Invoked OnBasicAttackPostHitEvent");
        }
        else
        {
            OnAttackPostHitEvent?.Invoke();
            //Debug.Log("Invoked OnAttackPostHitEvent");
        }
    }
}

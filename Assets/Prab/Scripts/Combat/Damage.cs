using Paraverse.Mob;
using Paraverse.Mob.Combat;
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
    [SerializeField, Tooltip("Applies hit animation to target.")]
    protected bool applyHitAnim = true;

    [Header("VFX")]
    public GameObject hitFX;

    [SerializeField]
    protected ScalingStatData scalingStatData;

    [SerializeField]
    protected bool isBasicAttackCollider = false;

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
    }
    #endregion

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
        float totalDmg = Mathf.CeilToInt(scalingStatData.FinalValue(mob.stats));
        controller.Stats.UpdateCurrentHealth(-(int)totalDmg);
        Debug.Log("Applied " + totalDmg + " points of damage to " + controller.Transform.name);
        return totalDmg;
    }

    protected virtual void DamageLogic(Collider other)
    {
        // Pre Basic Attack Hit Event
        if (isBasicAttackCollider)
        {
            OnBasicAttackPreHitEvent?.Invoke();
            Debug.Log("PROJ: Invoked OnBasicAttackPreEvent");
        }
        else
        {
            OnAttackPreHitEvent?.Invoke();
            Debug.Log("PROJ: Invoked OnAttackPreEvent");
        }

        IMobController controller = other.GetComponent<IMobController>();
        if (null != controller)
        {
            float dmg = ApplyCustomDamage(controller);

            // On Damage Applied Event
            if (isBasicAttackCollider)
            {
                OnBasicAttackApplyDamageEvent?.Invoke(dmg);
                Debug.Log("PROJ: Invoked OnBasicAttackApplyDamageEvent");
            }
            else
            {
                OnAttackApplyDamageEvent?.Invoke(dmg);
                Debug.Log("PROJ: Invoked OnAttackApplyDamageEvent");
            }

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

        if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

        // Post Basic Attack Hit Event
        if (isBasicAttackCollider)
        {
            OnBasicAttackPostHitEvent?.Invoke();
            Debug.Log("PROJ: Invoked OnBasicAttackPostHitEvent");
        }
        else
        {
            OnAttackPostHitEvent?.Invoke();
            Debug.Log("PROJ: Invoked OnAttackPostHitEvent");

        }
    }

    protected void InvokePreHitEvent()
    {
        // Pre Basic Attack Hit Event
        if (isBasicAttackCollider)
        {
            OnBasicAttackPreHitEvent?.Invoke();
            Debug.Log("Invoked OnBasicAttackPreEvent");
        }
        else
        {
            OnAttackPreHitEvent?.Invoke();
            Debug.Log("Invoked OnAttackPreEvent");
        }
    }

    protected void InvokeApplyDamageEvent(float dmg)
    {
        // On Damage Applied Event
        if (isBasicAttackCollider)
        {
            OnBasicAttackApplyDamageEvent?.Invoke(dmg);
            Debug.Log("Invoked OnBasicAttackApplyDamageEvent");
        }
        else
        {
            OnAttackApplyDamageEvent?.Invoke(dmg);
            Debug.Log("Invoked OnAttackApplyDamageEvent");
        }
    }

    protected void InvokePostHitEvent()
    {
        // Post Basic Attack Hit Event
        if (isBasicAttackCollider)
        {
            OnBasicAttackPostHitEvent?.Invoke();
            Debug.Log("Invoked OnBasicAttackPostHitEvent");
        }
        else
        {
            OnAttackPostHitEvent?.Invoke();
            Debug.Log("Invoked OnAttackPostHitEvent");

        }
    }
}

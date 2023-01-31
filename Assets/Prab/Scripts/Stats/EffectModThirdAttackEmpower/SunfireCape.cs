using Paraverse.Mob;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using System.Collections.Generic;
using UnityEngine;

public class SunfireCape : MonoBehaviour
{
    private MobStats _stats;
    private PlayerCombat _combat;

    [SerializeField]
    private string targetTag = StringData.EnemyTag;
    private float timer = 0f;
    private bool applyHit = false;
    private float attackPerUnitOfTime = 1f;
    private List<GameObject> hitTargets = new List<GameObject>();

    [Header("Effect Properties")]
    [SerializeField]
    private CapsuleCollider _col;
    [SerializeField]
    private float _effectRadius = 3f;

    [Header("Damage Properties")]
    [SerializeField]
    private ScalingStatData _empoweredScaling;

    private void Awake()
    {
        _stats = GameObject.FindGameObjectWithTag(StringData.PlayerTag).GetComponent<MobStats>();
        _combat = _stats.GetComponent<PlayerCombat>();
    }

    private void OnEnable()
    {
        _col = _stats.gameObject.AddComponent<CapsuleCollider>();
        _col.radius = _effectRadius;
        _col.isTrigger = true;
    }

    private void Update()
    {
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

    /// <summary>
    /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
    /// </summary>
    public float ApplyCustomDamage(IMobController controller)
    {
        float totalDmg =
            _empoweredScaling.FinalValue(_stats);

        controller.Stats.UpdateCurrentHealth(-Mathf.CeilToInt(totalDmg));
        return totalDmg;
    }

    private void DamageLogic(Collider other)
    {
        hitTargets.Add(other.gameObject);

        // Enemy-related logic
        if (other.TryGetComponent(out IMobController controller))
        {
            // Apply damage
            ApplyCustomDamage(controller);
        }

        // General VFX logic
        //if (hitFX) Instantiate(hitFX, other.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

        Debug.Log(other.name + " took " + _stats.AttackDamage.FinalValue + " points of damage.");
    }
}

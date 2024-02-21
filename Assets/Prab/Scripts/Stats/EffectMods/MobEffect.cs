using Paraverse.Mob;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using ParaverseWebsite.Models;
using System.Collections.Generic;
using UnityEngine;


public abstract class MobEffect : MonoBehaviour
{
  [SerializeField]
  protected string EffectName = "Mob Effect";
  // required to identify effect in database
  public EffectName EffectNameDB => _effectNameDB;
  protected EffectName _effectNameDB;
  protected MobStats _stats;
  protected PlayerCombat _combat;

  public int effectLevel = 1;

  [SerializeField]
  protected string targetTag = StringData.EnemyTag;
  public int ID { get { return _ID; } set { _ID = value; } }
  [Tooltip("Effect ID")]
  protected int _ID = -1;

  protected List<GameObject> hitTargets = new List<GameObject>();

  [SerializeField]
  protected GameObject effectFX;
  [SerializeField]
  protected GameObject hitFX;

  public bool IsActive => isActive; 
  protected bool isActive = false;
  protected GameObject _FX;
  
  public ScalingStatData[] scalingStatData;


  public virtual void EffectTick()
  {

  }

  public ScalingStatData GetScalingStatData(int level = -1)
  {
        // so that we don't accidentally request a level that's beyond the highest level allowed
        level = Mathf.Min(level, scalingStatData.Length - 1); 
        return (level != -1 ? scalingStatData[level - 1] : scalingStatData[effectLevel - 1]);
  }

  public virtual void ActivateEffect(MobStats stats, int id, int level)
  {
    ID = id;
    effectLevel = level;
    _stats = stats;
    _combat = _stats.GetComponent<PlayerCombat>();
    isActive = true;
  }

  public virtual void DeactivateEffect()
  {
    isActive = false;

    // Destroys instantiated FX
    _combat.Effects.Remove(this);
    if (_FX) Destroy(_FX.gameObject);
    Destroy(gameObject);
  }

  public virtual void AddSubscribersToSkillEvents(Damage col)
  {

  }

  public virtual void RemoveSubscribersToSkillEvents(Damage col)
  {

  }

  public virtual void OnSkillChangeApplyEffect()
  {

  }

  public virtual void OnEnemyDeathApplyEffect(MobController enemy)
  {
    // dont apply effect to players OnDeath() method
    if (enemy.tag.Equals(StringData.PlayerTag)) return;
  }

  /// <summary>
  /// useCustomDamage needs to be set to true on AttackCollider.cs inorder to apply this.
  /// </summary>
  public virtual float ApplyCustomDamage(IMobController controller)
  {
    float totalDmg = scalingStatData[effectLevel-1].FinalValueWithBoosts(_stats);

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

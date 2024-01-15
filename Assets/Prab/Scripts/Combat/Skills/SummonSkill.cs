using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using System.Collections;
using UnityEngine;

public class SummonSkill : MobSkill, IMobSkill
{
  #region Variables
  [Header("Summon Skill")]
  [SerializeField]
  protected float minSpawnPosX, maxSpawnPosX, minSpawnPosZ, maxSpawnPosZ;
  [SerializeField]
  protected GameObject summonPf;
  [SerializeField]
  protected int maxSummonCount = 3;
  protected int curSummonCount;
  [SerializeField]
  protected float delaySpawnActivation;
  [Header("VFX")]
  public GameObject launchFX;

  protected MobController _summonedMob;
  #endregion


  #region Inherited Methods
  public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
  {
    base.ActivateSkill(mob, anim, stats, target);
    mob.OnSummonSkillOneEvent += SummonSapling;
    SubscribeAnimationEventListeners();
  }

  protected override bool CanUseSkill()
  {
    if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false && anim.GetBool(StringData.IsBasicAttacking) == false && mob.ActiveSkill == null && CanSpawn())
      return true;
    return false;
  }

  protected override bool IsInRange()
  {
    return true;
  }
  #endregion

  #region Private Methods
  private void DecrementSummonCount(Transform target)
  {
    if (curSummonCount <= 0)
    {
      Debug.LogError("curSummonCount is already at 0...");
      return;
    }

    --curSummonCount;
  }

  protected bool CanSpawn()
  {
    return curSummonCount < maxSummonCount;
  }
  #endregion

  #region Animation Events
  public void SummonSapling()
  {
    if (summonPf == null)
      Debug.LogError("Please add a summon prefab to the skill: " + _skillName);

    // Get random position around mob
    float posX = Random.Range(minSpawnPosX, maxSpawnPosX);
    float posZ = Random.Range(minSpawnPosZ, maxSpawnPosZ);
    Vector3 spawnPos = mob.transform.position + new Vector3(posX, 0.25f, posZ);
    ++curSummonCount;

    StartCoroutine(IDelayedSpawn(() =>
    {
      GameObject sapling = Instantiate(summonPf, spawnPos, transform.rotation);
      _summonedMob = sapling.GetComponentInChildren<MobController>();
      _summonedMob.OnDeathEvent += DecrementSummonCount;
      SetSkillState(SkillState.InActive);
    },
    delaySpawnActivation));

    if (launchFX) Instantiate(launchFX, spawnPos, Quaternion.identity);

  }

  private IEnumerator IDelayedSpawn(System.Action a, float f)
  {
    yield return new WaitForSeconds(f);
    a?.Invoke();
    OnSkillComplete();
  }
  #endregion
}
